## Union Reporting Autotest Project

Гибридная автоматизация E2E‑тестирования локального портала **Union Reporting** (UI + API) с использованием Page Object Model, Aquality Selenium, NUnit, Selenium WebDriver и RestSharp.

---

## Описание

Проект содержит автоматизированный сценарий для портала Union Reporting, который разворачивается локально через Docker.

- **API** используется для: получения токена варианта, получения списка тестов проекта Nexage, создания теста, добавления лога и вложения (screenshot).
- **UI** используется для: авторизации по Basic Auth, передачи токена через cookie `token`, проверки номера варианта в футере, сравнения UI‑таблицы Nexage с API, создания проекта через `+Add` в новой вкладке, проверки появления теста и валидации данных на странице теста.

Все проверки выполняются через **NUnit Assert** в тестовом методе, в сценарии присутствует логирование шагов 1–6.

---

## Как запустить проект

### 1) Поднять тестируемое приложение (Docker)

1. Поднять контейнеры из пакета `docker-kits` (из репозитория экзамена).
2. Проверить доступность:
   - Web: `http://localhost:8080/web/`
   - API: `http://localhost:8080/api/`

### 2) Требования к окружению

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Установленный браузер (Chrome/Firefox)
- Запущенный Docker‑стенд на `localhost:8080`

### 3) Конфигурация проекта

Основные настройки лежат в `UnionReportingApi/appsettings.json`:

- `BaseUrl`: `http://localhost:8080/web/`
- `ApiBaseUrl`: `http://localhost:8080/api/`
- `Login`: `login`
- `Password`: `password`
- `VariantId`: `2`

> При необходимости можно создать `UnionReportingApi/appsettings.local.json` (файл в `.gitignore`) и переопределить параметры локально.

Настройки Aquality Selenium: `UnionReportingApi/Resources/settings.json`  
Тестовые данные и таймауты: `UnionReportingApi/Resources/testdata.json`

### 4) Сборка

```bash
dotnet restore UnionReportingApi.sln
dotnet build UnionReportingApi.sln
```

### 5) Запуск теста

```bash
dotnet test UnionReportingApi.sln --filter "FullyQualifiedName~UnionReportingVariant2AllStepsShouldPass"
```

---

## Структура проекта

```
UnionReportingApi/
├── UnionReportingApi/
│   ├── Tests/                  # E2E сценарий и базовая инфраструктура тестов
│   ├── Steps/                  # Бизнес-шаги (логин, cookie token, создание проекта)
│   ├── Forms/                  # Page Object — взаимодействие с UI
│   ├── Api/                    # Клиент Union Reporting API + модели
│   ├── Constants/              # ApiConstants/WebConstants (endpoints, параметры, подписи полей и т.п.)
│   ├── Utils/                  # Утилиты (ожидания)
│   ├── TestData/               # Загрузка testdata.json
│   ├── Helpers/                # Вкладки, сборка URL с Basic Auth
│   ├── Config/                 # Загрузка appsettings.json
│   ├── Resources/
│   │   ├── settings.json       # Настройки браузера Aquality Selenium
│   │   └── testdata.json       # Данные/таймауты сценария
│   ├── appsettings.json        # Настройки стенда (URL/креды/вариант)
│   ├── appsettings.local.json  # Локальные переопределения (не в git)
│   └── NLog.config             # Настройки логирования
├── UnionReportingApi.sln
└── README.md
```

---

## Описание ключевых компонентов

### 1) UI (Aquality Selenium) + Page Object Model

- Доступ к браузеру: `AqualityServices.Browser`
- Формы (Page Object): `ProjectsForm`, `AllTestsForm`, `AddProjectForm`, `TestInfoForm`
- Ожидания для AJAX: `ConditionalWaitHelper`

### 2) Steps

- `LoginSteps` — открытие страниц через Basic Auth
- `AuthSteps` — установка cookie `token` и проверка футера `Version: 2`
- `ProjectSteps` — создание проекта через `+Add` (новая вкладка)

### 3) REST API

- `ReportingApiClient` — запросы через RestSharp (Basic Auth), возвращает `RestResponse`
- `TryGetProjectTestsFromJson/Xml` — получение списка тестов с retry по формату ответа
- Модели: `ProjectTestItem`, `CreateTestRequest`, `AttachmentContent`

### 4) Константы и конфиги

- `ApiConstants` — эндпоинты, query/form параметры, протокольные значения (media types, XML tags)
- `WebConstants` — cookie, пути страниц, подписи полей на странице теста, HTML‑атрибуты/теги
- `TestConfiguration` — доступ к `appsettings.json`
- `TestDataProvider` — доступ к `testdata.json`

---

## Реализованные тесты

| # | Класс | Метод | Описание |
|---|---|---|---|
| 1 | `UnionReportingVariant2Scenario` | `UnionReportingVariant2AllStepsShouldPass` | Полный гибридный сценарий Variant 2 (6 шагов: token → cookie → Nexage UI/API → +Add project → API test/log/attachment → проверка testInfo) |

---

## Технологии

| Компонент | Использование |
|---|---|
| **C# / .NET 8** | Язык и платформа |
| **NUnit** | Тестовый фреймворк |
| **Aquality Selenium** | Обёртка над Selenium WebDriver |
| **Selenium WebDriver** | Автоматизация браузера |
| **RestSharp** | HTTP‑запросы к Union Reporting API |
| **Microsoft.Extensions.Configuration** | Чтение `appsettings.json` |

