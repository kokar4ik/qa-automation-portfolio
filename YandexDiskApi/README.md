# Yandex Disk Autotest Project

Гибридная автоматизация E2E-тестирования [Яндекс.Диска](https://disk.yandex.ru/) с использованием Page Object Model, Aquality Selenium, NUnit, Selenium WebDriver, RestSharp и Yandex Disk REST API.

---

## Описание

Проект содержит автоматизированный сценарий для Яндекс.Диска: операции с файлами выполняются через REST API, проверки интерфейса — через браузер.

Тест проходит полный цикл: загрузка текстового файла, просмотр содержимого, переименование в изображение, загрузка JPEG, просмотр превью, удаление в корзину через drag-and-drop, проверка корзины через API, восстановление и безвозвратное удаление с проверкой в UI.

Валидации выполняются через NUnit Assert, взаимодействие с браузером — через корпоративный фреймворк Aquality Selenium.

---

## Как запустить проект

1. Клонировать репозиторий и перейти в корневую папку решения.
2. Убедиться, что установлен [.NET 8 SDK](https://dotnet.microsoft.com/download) и Google Chrome.
3. Создать файл `YandexDiskApi/appsettings.local.json` на основе `appsettings.json` и заполнить секреты:

```json
{
  "YandexDisk": {
    "BaseUrl": "https://disk.yandex.ru/",
    "ApiBaseUrl": "https://cloud-api.yandex.net/v1/",
    "OAuthToken": "ваш_oauth_токен",
    "Login": "ваш_логин",
    "Password": "ваш_пароль"
  }
}
```

> Файл `appsettings.local.json` не должен попадать в репозиторий (содержит токен и пароль).

4. Проверить конфигурацию браузера в `YandexDiskApi/Resources/settings.json` и тестовые данные в `YandexDiskApi/Resources/testdata.json`.
5. Положить тестовое изображение в `YandexDiskApi/Resources/test-image.jpg` (путь указан в `testdata.json`).
6. Восстановить зависимости и собрать проект:

```bash
dotnet restore YandexDiskApi.sln
dotnet build YandexDiskApi.sln
```

7. Запустить тест:

```bash
dotnet test YandexDiskApi.sln --filter "FullyQualifiedName~YandexDiskFullScenario"
```

Или через Test Explorer в Visual Studio / Rider — тест помечен атрибутом `[Explicit]`, поэтому его нужно запустить **явно**.

> При входе в аккаунт может потребоваться **ручное подтверждение 2FA** (код из SMS или приложения) в открытом браузере.

---

## Структура проекта

```
YandexDiskApi/
├── YandexDiskApi/
│   ├── Tests/                  # Тестовые сценарии и базовая инфраструктура
│   ├── Steps/                  # Бизнес-шаги (логин и т.д.)
│   ├── Forms/                  # Page Object — взаимодействие с UI
│   ├── Api/                    # Клиент Yandex Disk REST API
│   ├── Constants/              # Константы API, URL, медиатипов
│   ├── Utils/                  # Утилиты (JS, ожидания, текст, пути)
│   ├── TestData/               # Загрузка testdata.json
│   ├── Helpers/                # Генерация данных, вкладки
│   ├── Config/                 # Загрузка конфигурации
│   ├── Resources/
│   │   ├── settings.json       # Настройки браузера Aquality Selenium
│   │   ├── testdata.json       # Расширения файлов и таймауты
│   │   └── test-image.jpg      # Тестовое изображение
│   ├── appsettings.json        # Шаблон настроек приложения
│   ├── appsettings.local.json  # Локальные секреты (не в git)
│   └── nlog.config             # Настройки логирования
├── YandexDiskApi.sln
└── README.md
```

---

## Описание ключевых компонентов

### 1. Управление браузером (Aquality Selenium)
- Централизованный доступ к браузеру через `AqualityServices.Browser`
- Настройки драйвера и таймаутов в `Resources/settings.json`
- Базовый класс `TestBase` — открытие страницы, создание API-клиента, закрытие браузера

### 2. Page Object Model
- Инкапсуляция локаторов и атомарных действий на страницах
- Наследование от `Form` (Aquality Selenium)
- Формы: `DiskMainForm`, `LoginForm`, `DocumentViewerForm`, `ImageViewerForm`

### 3. Steps
- `LoginSteps` — сценарий входа с ручным подтверждением 2FA

### 4. REST API (Yandex Disk)
- `YandexDiskApiClient` — HTTP-запросы через RestSharp, возвращает `RestResponse`
- OAuth-токен и базовый URL API из конфигурации
- Проверки статусов и контента — в тестовом методе через `Assert`

### 5. Вспомогательные классы
- `BrowserTabsHelper` — запоминание основной вкладки, переключение и закрытие
- `TestDataGenerator` — случайное имя файла и текст
- `TestDataProvider` — загрузка `testdata.json`
- `JsExecutor`, `ConditionalWaitHelper`, `TextUtils`, `DiskPathUtils`

### 6. Тестовый фреймворк
- **NUnit** — организация и запуск тестов
- **Assert** — проверка ожидаемых результатов
- **Explicit** — тест требует ручного участия при 2FA

### 7. Конфигурирование
- `Resources/settings.json` — браузер, таймауты Aquality
- `Resources/testdata.json` — расширения файлов и таймауты сценария
- `appsettings.json` / `appsettings.local.json` — URL диска, URL API, OAuth, логин, пароль
- `TestConfiguration` — единая точка доступа к настройкам

---

## Реализованные тесты

| # | Класс | Метод | Описание |
|---|---|---|---|
| 1 | `YandexDiskE2ETest` | `YandexDiskFullScenario` | Полный гибридный сценарий: API + UI (загрузка, просмотр, переименование, JPEG, корзина, восстановление, удаление) |

### Шаги сценария `YandexDiskFullScenario`

| Шаг | Канал | Действие |
|---|---|---|
| 1 | UI | Вход в Яндекс.Диск (логин/пароль из конфига, 2FA вручную) |
| 2 | API | Загрузка `.txt` на диск |
| 3 | UI | Ожидание файла в списке, открытие и проверка текста в viewer |
| 4 | API | Переименование `.txt` → `.jpg` |
| 5 | API + UI | Загрузка JPEG поверх файла, проверка отображения в списке |
| 6 | UI | Открытие превью изображения |
| 7 | UI + API | Drag-and-drop в корзину, проверка наличия в корзине через API |
| 8 | API + UI | Восстановление из корзины, проверка в списке |
| 9 | API + UI | Безвозвратное удаление, обновление страницы, проверка отсутствия файла |

> После безвозвратного удаления UI Яндекс.Диска может не обновить список файлов без перезагрузки страницы. Перед финальной проверкой выполняется `RefreshPage()`.

---

## Технологии

| Компонент | Использование |
|---|---|
| **C# / .NET 8** | Язык и платформа |
| **NUnit** | Тестовый фреймворк |
| **Aquality Selenium** | Корпоративная обёртка над Selenium WebDriver |
| **Selenium WebDriver** | Автоматизация браузера |
| **RestSharp** | Все HTTP-запросы к Yandex Disk API и загрузка/скачивание по ссылкам |
| **Microsoft.Extensions.Configuration** | Чтение `appsettings.json` |

---

## Требования к окружению

- .NET 8 SDK
- Google Chrome (последняя стабильная версия)
- Аккаунт Яндекс с доступом к Диску
- OAuth-токен Yandex Disk API
- Доступ в интернет

---
