# BackendOfTheProject
# EgorkaCoins

EgorkaCoins - это учебный онлайн-магазин для покупки игровой валюты и связанных пакетов. Проект объединяет backend на .NET и frontend на React, а также включает авторизацию, корзину, оплату, отзывы, жалобы, личный кабинет, модерацию и административные инструменты.

## Скриншоты
Главная страница
<img width="1898" height="921" alt="Pasted image 20260604164050" src="https://github.com/user-attachments/assets/78fc7288-f360-41e8-9887-c7e8d0ce5d76" />
Каталог и выбор игры
<img width="1898" height="921" alt="Pasted image 20260604164452" src="https://github.com/user-attachments/assets/7f32b029-990e-460b-a6c4-30f27aa7b521" />
Страница товара/пакета
<img width="1898" height="918" alt="Pasted image 20260604164618" src="https://github.com/user-attachments/assets/8b88d9a5-9c01-4670-82c4-01a8fd5eb047" />
Корзина
<img width="1910" height="919" alt="Pasted image 20260604164732" src="https://github.com/user-attachments/assets/1f54db96-1c9a-4810-b064-e9208d2fce5a" />
Оформление покупки
<img width="1901" height="921" alt="Pasted image 20260604164826" src="https://github.com/user-attachments/assets/a4d4ef94-e553-4044-900f-ff391c6214c6" />
Личный кабинет
<img width="1906" height="920" alt="Pasted image 20260604164856" src="https://github.com/user-attachments/assets/68c3c89c-63c4-48f9-b43d-da8eecd5b605" />
Панель админа
<img width="1910" height="919" alt="Pasted image 20260604164920" src="https://github.com/user-attachments/assets/de94497a-b438-408c-b7b0-5da1e10463ab" />
Панель модератора
<img width="1896" height="919" alt="Pasted image 20260604164947" src="https://github.com/user-attachments/assets/fd177f5d-c060-44f1-ba36-89d0b98a0892" />
Отзывы и жалобы
<img width="1890" height="917" alt="Pasted image 20260604165012" src="https://github.com/user-attachments/assets/8aca5e3c-bfc0-4738-872e-c0c97e7838a8" />
<img width="941" height="678" alt="Pasted image 20260604165030" src="https://github.com/user-attachments/assets/6b4ba805-65b0-4337-8010-60b5c577c7a4" />
<img width="927" height="471" alt="Pasted image 20260604165051" src="https://github.com/user-attachments/assets/c1a88629-6dc7-4ac4-90d0-166692a36cc1" />

## Технологии
### Backend

- `.NET 10`
- `ASP.NET Core Web API`
- `Entity Framework Core`
- `SQL Server`
- `JWT` авторизация
- `AutoMapper`
- `Swagger / OpenAPI`

### Frontend

- `React 18`
- `TypeScript`
- `Vite`
- `React Router`
- `MUI`
- `Radix UI`
- `Tailwind CSS`
- `Motion`

## Возможности

- каталог игр и пакетов
- корзина и оформление заказа
- оплата картой и криптовалютой в учебном формате
- регистрация, вход и личный профиль
- личный кабинет со статистикой покупок
- система отзывов
- система жалоб для модерации
- роли `user`, `moderator` и `admin`
- статистика и управление пользователями для админа
- автоматическое начисление опыта после успешной покупки

## Архитектура

Backend разделен на простые слои:

- `EgorkaCoins.Api` - контроллеры, фильтры, auth-сервисы и API-контракты
- `EgorkaCoins.BusinessLogic` - бизнес-логика для пользователей, каталога, покупок и жалоб
- `EgorkaCoins.DataAccess` - `AppDbContext`, подключение к БД и миграции
- `EgorkaCoins.Domain` - сущности базы данных
- `EgorkaCoins.Helpers` - DTO и вспомогательные классы

Frontend представляет собой SPA с:

- переиспользуемыми UI-компонентами
- отдельными страницами для каталога, корзины, оплаты, кабинета, отзывов и авторизации
- локальным хранением корзины
- API-обертками для общения с backend

## Структура проекта

```text
EgorkaCoins.slnx
EgorkaCoins.Api/
EgorkaCoins.BusinessLogic/
EgorkaCoins.DataAccess/
EgorkaCoins.Domain/
EgorkaCoins.Helpers/
BACKEND_STRUCTURE.md
```

## Требования

- `.NET 10 SDK`
- `Node.js 20+`
- `npm`
- `SQL Server`

## Установка backend

1. Открой solution из корня репозитория.
2. Проверь строку подключения и JWT-настройки в `EgorkaCoins.Api/appsettings.json`.
3. Восстанови пакеты и примени миграции:

```bash
dotnet restore EgorkaCoins.slnx
dotnet ef database update --project EgorkaCoins.DataAccess --startup-project EgorkaCoins.Api
```

4. Запусти API:

```bash
dotnet run --project EgorkaCoins.Api
```

По умолчанию API поднимается на:

- `http://localhost:5152`
- `https://localhost:7023`

## Установка frontend

1. Открой папку `D:\ProjectOnlineShop\FrontendOfTheProject`.
2. Установи зависимости:

```bash
npm install
```

3. При необходимости создай `.env.local` и укажи адрес API:

```bash
VITE_API_URL=https://localhost:7023
```

4. Запусти frontend:

```bash
npm run dev
```

Frontend работает через Vite и обращается к backend по адресу из `VITE_API_URL`.

## Конфигурация

### Backend

Файл настроек: `EgorkaCoins.Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EgorkaCoins;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Issuer": "EgorkaCoinsApi",
    "Audience": "EgorkaCoinsClients",
    "Key": "your-secret-key",
    "ExpireMinutes": "60"
  }
}
```

### Frontend

Файл настроек: `.env.local`

```bash
VITE_API_URL=https://localhost:7023
```

Если локально используешь HTTP, подставь тот порт и протокол, на котором реально запущен API.

## Примечания

- Авторизация основана на `JWT` и ролях.
- Жалобы рассматривает модератор, а админ имеет доступ ко всем тем же функциям и к административным разделам.
- Опыт начисляется автоматически после успешной покупки и показывается в личном кабинете.
- Код разложен по простым папкам, чтобы проект было легко читать и развивать как учебную работу.
