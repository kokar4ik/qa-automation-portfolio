# JsonPlaceholder API Autotest Project

Автоматизация API-тестирования [jsonplaceholder.typicode.com](https://jsonplaceholder.typicode.com/) и интеграция с MySQL (задание по БД) с использованием NUnit, HttpClient, FluentAssertions, MySqlConnector, System.Text.Json.

---

## Описание

Проект содержит:

- API-тест для fake REST API JSONPlaceholder (6 шагов: GET/POST к `posts` и `users`).
- Интеграционные тесты задания по БД: **ТК1** (запись результата теста в MySQL) и **ТК2** (копирование, мок-запуск, обновление, удаление).

HTTP-запросы — через `ApiClient`, бизнес-методы — через `PostsApi` и `UsersApi`.  
Работа с MySQL — через репозитории (`TestRepository` и др.) и SQL через `MySqlConnector`.

---

## Требования к окружению

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL 8 (локальная установка, без Docker)
- Доступ в интернет (API-тесты обращаются к публичному API)

---

## Настройка MySQL

1. Установить **MySQL Server 8** (Server only).
2. Скачать дамп: [db_task_dump](https://github.com/tquality-education/db_task_dump).
3. Создать базу и импортировать дамп:

```cmd
"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe" -u root -p -e "CREATE DATABASE IF NOT EXISTS union_reporting CHARACTER SET utf8;"

cd путь\к\папке\с\dump.sql
"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe" -u root -p union_reporting < dump.sql
```

4. Проверить импорт:

```cmd
"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe" -u root -p -e "USE union_reporting; SELECT COUNT(*) FROM test;"
```

---

## Настройка appsettings.json

Файл: `JsonPlaceholderApiTests/appsettings.json`.

Секция `Database` — параметры подключения к локальному MySQL:

```json
"Database": {
  "Host": "localhost",
  "Port": 3306,
  "Database": "union_reporting",
  "User": "root",
  "Password": ""
}
```

Перед запуском тестов укажите свой пароль пользователя `root` локально в поле `Password`.

Секция `DatabaseTestData` — тестовые данные для записей в БД (проект, автор, лимиты выборок и длительности; можно изменить под другое решение):

```json
"DatabaseTestData": {
  "ProjectName": "JsonPlaceholderApiTests",
  "AuthorName": "Ermoshkin",
  "AuthorLogin": "z.ermoshkin",
  "AuthorEmail": "z.ermoshkin@example.com",
  "RepeatingDigitTestsLimit": 10,
  "CopySourceTestsLimit": 1,
  "CreateTestDurationSeconds": 2,
  "UpdateTestDurationSeconds": 10
}
```

---

## Как запустить проект

1. Клонировать репозиторий и перейти в корневую папку решения.
2. Выполнить настройку MySQL (см. выше).
3. Указать пароль MySQL в `appsettings.json`.
4. Восстановить зависимости и собрать проект:

```bash
dotnet restore JsonPlaceholderApiTests.sln
dotnet build JsonPlaceholderApiTests.sln
```

5. Запустить все тесты:

```bash
dotnet test JsonPlaceholderApiTests.sln
```

Только тесты задания по БД:

```bash
dotnet test JsonPlaceholderApiTests.sln --filter "FullyQualifiedName~DbTask"
```

---

## Тест-кейсы задания по БД

| Тест | Файл | Что проверяет |
|------|------|----------------|
| ТК1 | `DbTaskTestCase1Tests.TestCase1RunApiScenarioAndSaveResultToDatabase` | API-сценарий проходит, результат записывается в таблицу `test` |
| ТК2 | `DbTaskTestCase2Tests.TestCase2CopySimulateRunUpdateAndCleanup` | Выборка тестов по id, копирование, мок-запуск, обновление, удаление копий |

Проверка записей в MySQL после ТК1:

```sql
USE union_reporting;

SELECT t.id, t.name, st.name AS status, t.start_time, t.end_time
FROM test t
LEFT JOIN status st ON st.id = t.status_id
JOIN project p ON p.id = t.project_id
WHERE p.name = '<значение DatabaseTestData:ProjectName из appsettings.json>'
ORDER BY t.id DESC
LIMIT 5;
```

---

## Структура проекта

```
JsonPlaceholderApiTests/
├── JsonPlaceholderApiTests/
│   ├── Tests/                  # API-тесты, DbTask ТК1/ТК2, TestBase
│   ├── Api/                    # PostsApi, UsersApi
│   ├── Database/               # DatabaseConnection, Models, Repositories, Utils
│   ├── Utils/                  # ApiClient, TestLogger, конфиг
│   ├── Models/                 # DTO для JSON API
│   ├── Configuration/          # ApiSettings, DatabaseSettings, TestDataSettings
│   ├── Constants/              # Ключи конфига
│   ├── Data/                   # Эталонные JSON-данные
│   └── appsettings.json
├── JsonPlaceholderApiTests.sln
└── README.md
```

---

## Описание ключевых компонентов

### API

- **ApiClient** — базовый HTTP-клиент
- **PostsApi / UsersApi** — методы для `posts` и `users`
- **JsonPlaceholderScenarioRunner** — общий сценарий из 6 шагов

### База данных

- **DatabaseSettings / DatabaseConnection** — конфиг и подключение к MySQL
- **Database/Models/*Record** — представления таблиц
- **TestStatus** — enum статусов теста (Passed, Failed, Skipped)
- **Database/Repositories/** — CRUD через SQL (MySqlConnector)
- **TestRunSimulator** — имитация запуска теста для ТК2

### Прочее

- **TestLogger (NLog)** — логи в `bin/Debug/net8.0/logs/test.log`
- **appsettings.json** — настройки API, тестовых данных и БД

---

## Технологии

| Компонент | Использование |
|---|---|
| **C# / .NET 8** | Язык и платформа |
| **NUnit** | Тестовый фреймворк |
| **HttpClient** | HTTP-запросы к API |
| **MySqlConnector** | Подключение к MySQL, SQL |
| **FluentAssertions** | Assert'ы |
| **Microsoft.Extensions.Configuration** | Загрузка appsettings.json |
| **System.Text.Json** | Сериализация/десериализация JSON |

---
