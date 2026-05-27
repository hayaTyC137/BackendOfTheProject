# Backend Structure

Бэкенд разложен по простому принципу: сначала проект, потом зона ответственности внутри проекта.

## Проекты

- `EgorkaCoins.Api` - HTTP API, контроллеры, фильтры, сервисы токенов и OAuth.
- `EgorkaCoins.BusinessLogic` - действия над сущностями: логин, покупки, жалобы, отзывы, заказы.
- `EgorkaCoins.DataAccess` - `AppDbContext`, миграции и подключение к БД.
- `EgorkaCoins.Domain` - сущности базы данных.
- `EgorkaCoins.Helpers` - DTO и вспомогательные классы.

## Папки внутри проектов

### `EgorkaCoins.Api`

- `Controllers/Auth` - вход, регистрация, текущий пользователь по токену.
- `Controllers/Catalog` - игры и пакеты.
- `Controllers/Commerce` - заказы и платежи.
- `Controllers/Community` - отзывы и жалобы.
- `Controllers/System` - health и статистика.
- `Controllers/Users` - профиль, аватар, управление пользователями.
- `Contracts/Users` - запросы, которые относятся только к API-слою.
- `Services/Auth` - JWT и OAuth-логика.
- `Filters/Auth` - кастомные auth-фильтры.

### `EgorkaCoins.BusinessLogic`

- `Core/Catalog` - действия для игр и пакетов.
- `Core/Commerce` - действия для заказов и платежей.
- `Core/Community` - действия для отзывов и жалоб.
- `Core/Users` - действия для пользователей.
- `Mappings` - AutoMapper profile.

### `EgorkaCoins.Domain`

- `Catalog` - `Game`, `Package`.
- `Commerce` - `Order`, `Payment`, `PaymentItem`.
- `Community` - `Review`, `Report`.
- `Users` - `User`.

### `EgorkaCoins.Helpers`

- `DTOs/Auth` - вход, регистрация, смена пароля.
- `DTOs/Commerce` - заказ, платеж, статус заказа.
- `DTOs/Community` - отзывы и жалобы.
- `DTOs/Users` - профиль, роли, бан, пользовательские DTO.
- `DTOs/Admin` - админская статистика.
- `Security` - хеширование пароля и похожие утилиты.

## Идея структуры

Если нужно найти код:

- работа с HTTP-запросом: смотри `EgorkaCoins.Api`
- бизнес-логика: смотри `EgorkaCoins.BusinessLogic`
- модель таблицы/сущности: смотри `EgorkaCoins.Domain`
- DTO для запроса или ответа: смотри `EgorkaCoins.Helpers`
- миграции и контекст БД: смотри `EgorkaCoins.DataAccess`
