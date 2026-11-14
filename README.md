# RePoint

**Тема на дипломната работа** — „Платформа за координация на разделно събиране и предаване на отпадъци“

**Автор** — Християн Емилов Беков, XII А клас, ЧПГДН „СофтУни Будител“, София

**Консултант** — Николай Палашев

**Първа контролна проверка** — 16.11.2026 г.

**Окончателно предаване** — 19.04.2027 г.

RePoint е монолитно уеб приложение с ASP.NET Core MVC, Razor, C#, .NET 10, ASP.NET Core Identity и EF Core Code First за Microsoft SQL Server. Контролерите и Razor изгледите са в `src/RePoint.Web`, а началните xUnit тестове са в `tests/RePoint.Tests`.

## Етап 1

Локално са проверени началната страница, каталогът на материалите, пунктовете с филтри по град и материал, подробностите за пункт, празният списък с кампании, регистрацията, входът, изходът и подаването и прегледът на собствени заявки. Демонстрационните пунктове са фиктивни. Проверките обхващат валидацията и персонализираните страници 404 и 500.

Миграцията `InitialCreate` създава схемата и началните записи за 5 материала, 2 пункта, 4 комбинации пункт–материал и 4 Identity роли.

| Роля | Планирани права |
| --- | --- |
| Потребител (`User`) | Профил, подаване и преглед на собствени заявки, сигнали |
| Оператор (`Operator`) | Обработка на заявки и потвърждение в обслужваните пунктове |
| Организатор (`Organizer`) | Управление на кампании и преглед на отчети |
| Администратор (`Administrator`) | Управление на потребители, роли, пунктове, материали и сигнали |

Текущите заявки изискват автентикация и са ограничени до собственика. Разпределянето на роли при регистрация и отделните права по роли предстоят. Предстоят също операторско потвърждение, отчети, карта, филтър по работно време, управление на кампании, сигнали, известия и публична демонстрация.

## Локално стартиране

Необходими са актуален [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0), [Git](https://git-scm.com/install/windows) и Microsoft SQL Server. Следващите команди се изпълняват в PowerShell от корена на проекта.

### SQL Server Express LocalDB под Windows

Инсталирай LocalDB чрез SQL Server Express, ако липсва. При тази конфигурация се използва Windows автентикация.

```powershell
sqllocaldb start MSSQLLocalDB
dotnet user-secrets set 'ConnectionStrings:RePointDb' 'Server=(localdb)\MSSQLLocalDB;Database=RePointDb;Integrated Security=True;Encrypt=True;TrustServerCertificate=True' --project src/RePoint.Web
```

### SQL Server 2022 в Docker

Инсталирай [Docker Desktop с WSL 2](https://docs.docker.com/desktop/setup/install/windows-install/) и го стартирай. Копирай `.env.example` в `.env` и задай собствена силна парола в `.env`. Портът е достъпен само на локалния интерфейс.

```powershell
Copy-Item .env.example .env
# Редактирай .env преди стартиране.
docker compose up -d
docker compose logs sqlserver
```

Изчакай съобщението, че SQL Server приема връзки. Въведи същата парола от `.env` в скритото поле на следващата команда. PowerShell подава връзката към User Secrets през стандартния вход, а `.env` е игнориран от Git.

```powershell
$sqlPassword = Read-Host 'Локална SQL парола' -MaskInput
$sqlConnection = [System.Data.Common.DbConnectionStringBuilder]::new()
$sqlConnection.ConnectionString = 'Server=localhost,14333;Database=RePointDb;User Id=sa;Encrypt=True;TrustServerCertificate=True'
$sqlConnection['Password'] = $sqlPassword
@{ 'ConnectionStrings:RePointDb' = $sqlConnection.ConnectionString } |
    ConvertTo-Json -Compress | dotnet user-secrets set --project src/RePoint.Web
Remove-Variable sqlPassword,sqlConnection
```

### Компилация и миграция

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet tool restore
dotnet restore RePoint.slnx
dotnet build RePoint.slnx -c Release --no-restore
dotnet ef database update --project src/RePoint.Web --startup-project src/RePoint.Web --configuration Release --no-build
dotnet dev-certs https --trust
dotnet run --project src/RePoint.Web -c Release --no-build --no-launch-profile --urls https://localhost:7246
```

Отвори [локалното приложение](https://localhost:7246). Миграцията е в `src/RePoint.Web/Data/Migrations/`. За проверка на таблиците и началните данни използвай [SQL инструкциите](db/README.md).

## Тестове и документация

```powershell
dotnet test RePoint.slnx -c Release --no-build --logger trx --collect:"XPlat Code Coverage" --results-directory TestResults
```

Workflow файлът конфигурира restore, Release build и unit тестове и качване на TRX и Cobertura като артефакт. Първото изпълнение в GitHub предстои. [Отчетът за Етап 1](E1_VERIFICATION.md) съдържа действителните локални резултати и текущото покритие. [Матрицата на изискванията](docs/requirements-traceability.md) свързва заданието с проверките и оставащата работа.

Публичното хранилище е [CnapKy/RePoint](https://github.com/CnapKy/RePoint). За предаване виж [информацията за Етап 1](docs/predavane-etap1.md).
