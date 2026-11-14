# Проверка на RePoint за Етап 1

**Дата** — 14.11.2026 г.

**Тема** — „Платформа за координация на разделно събиране и предаване на отпадъци“

**Основание** — предоставените изображения на индивидуалното задание и работният `RePoint_Documentation.docx`.

Проверен е съществуващият код от архива за Етап 1 върху историята на `CnapKy/RePoint` от `8a2a061`, в локален клон `etap-1-verification`. Посоченият път `C:\Projects\RePoint` липсваше в достъпната среда. Работното копие, NuGet кешът и SQL файловете са на `D:\RePoint-Etap1`, тъй като диск `C:` беше запълнен. Оригиналният DOCX е отделен от файловете за публикуване. Снимките показват текста на заданието и празни полета за подписи.

## Среда и команди

| Команда | Резултат |
| --- | --- |
| `dotnet --info` | .NET SDK 10.0.401, ASP.NET Core runtime 10.0.12, Windows x64 |
| `git --version` | 2.56.0.windows.4 |
| `dotnet ef --version` | Първоначално липсваше. След `dotnet tool restore` от включения манифест — 10.0.15 |
| `docker version`, `wsl --list --quiet` | Docker CLI и WSL липсват. Проверката на базата е извършена чрез наличния LocalDB |
| `sqllocaldb start MSSQLLocalDB`, `sqlcmd -E` | Успешно. SQL Server 2019 Express LocalDB 15.0.4153.1 |
| `dotnet user-secrets set` | Записана локална връзка с Windows автентикация |
| `dotnet restore RePoint.slnx` | Успешно след пренасочване на NuGet и временните файлове към `D:` и обновяване на пакетите |
| `dotnet build RePoint.slnx -c Release --no-restore` | Успешно, 0 грешки и 0 предупреждения |
| `dotnet test RePoint.slnx -c Release --no-build --logger trx --collect:"XPlat Code Coverage" --results-directory TestResults/E1` | 12 успешни, 0 неуспешни, 0 пропуснати. Записани TRX и Cobertura |
| `dotnet list RePoint.slnx package --vulnerable --include-transitive` | NuGet не отчете уязвими пакети за двата проекта към момента на проверката |
| `dotnet ef migrations add InitialCreate` | Генерирани миграция, designer и snapshot в `src/RePoint.Web/Data/Migrations/` |
| `dotnet ef database update` | Приложена `20261113234444_InitialCreate`. Повторното изпълнение потвърди актуална схема |
| `dotnet ef migrations has-pending-model-changes` | Няма промени спрямо последната миграция |
| `sqlcmd -S '(localdb)\MSSQLLocalDB' -E -b -d RePointDb -i 'db\verification.sql' -f 65001` | Прочетена физическата схема и проверени началните данни |
| `dotnet dev-certs https --check --trust`, `dotnet run --project src/RePoint.Web -c Release --no-build --no-launch-profile --urls https://localhost:7246` | Доверен сертификат и стартирал Kestrel по HTTPS |
| `actionlint -no-color .github/workflows/ci.yml` | Успешно с actionlint 1.7.12. GitHub runner е проверен при финалния преглед |
| `git diff --cached --check`, `git push --dry-run origin HEAD:refs/heads/etap-1-verification` | Подготвените файлове преминаха проверката. Dry run потвърди достъп до отдалеченото хранилище. LICENSE съвпада с версията в `origin/main` |

EF командите са изпълнени с `--project src/RePoint.Web --startup-project src/RePoint.Web --configuration Release --no-build`. При генерирането е зададен и `--output-dir Data/Migrations`. Празната LocalDB база `RePointDb` е създадена с файлове на `D:`, а EF Core е създал таблиците и началните записи.

При запълнен системен диск настройката за тази проверка беше:

```powershell
$env:NUGET_PACKAGES = 'D:\RePoint-Etap1\packages'
$env:NUGET_HTTP_CACHE_PATH = 'D:\RePoint-Etap1\nuget-cache'
$env:TEMP = 'D:\RePoint-Etap1\temp'
$env:TMP = $env:TEMP
New-Item -ItemType Directory -Force $env:TEMP | Out-Null
```

## Схема и роли

- SQL Server отчете **16 таблици** — 8 домейн таблици, 7 Identity таблици и `__EFMigrationsHistory`. Проверени са 16 първични и 18 външни ключа, включително съставния ключ на `PointMaterials` и съставния външен ключ от заявката към тази таблица.
- Потвърдени са 3 `CHECK` ограничения за приблизително количество, действително количество и дати на кампаниите. Уникалните индекси върху `Materials.Code` и `ConfirmedDeliveries.HandoverRequestId` имат `is_unique = 1`. В отменена транзакция SQL Server отхвърли дублирано потвърждение с грешка 2601 и нулево приблизително количество с грешка 547.
- Потвърдени са 5 материала, 2 фиктивни пункта, 4 комбинации пункт–материал и 4 Identity роли. В отменена транзакция един тестов акаунт беше свързан успешно с `User`, `Operator`, `Organizer` и `Administrator`. Отделни права по роли и присвояване на роля при регистрация предстоят.
- Кодът съдържа 9 Entity класа, включително `ApplicationUser`, 5 ViewModels, 5 MVC контролера и 9 MVC страници, включително 404 и 500. Броят на страниците изключва общия layout, служебните Razor файлове и вградените Identity страници.

## Проверени сценарии

| Сценарий | Резултат |
| --- | --- |
| Начало, CSS, материали | HTTPS 200. Началната страница показва 2 пункта и 5 материала, а каталогът показва петте материала |
| Пунктове | Списък с двата пункта, комбиниран филтър за София и материал, празен резултат за друг град, детайли и приемани материали |
| Кампании | HTTPS 200 и съобщение при празен списък |
| Регистрация и вход | Слаба парола е отхвърлена. Два локални акаунта се регистрираха. Проверени грешна и вярна парола, изход и Secure/HttpOnly cookie |
| Достъп и заявки | Неавтентикиран потребител получи 302 към входа. Записана е заявка от 12.345 кг със статус `Submitted`. Друг акаунт не вижда заявката |
| Сървърна валидация | Отхвърлени отрицателно количество, четири десетични знака и материал, който пунктът не приема |
| Браузър | Chromium 159, ширина 390 px. Проверени навигация, регистрация и HTML `min`, `max`, `step`. Валидната гранична стойност 0.001 кг е записана |
| Грешки | Несъществуващи URL и пункт показват персонализирана 404 със статус 404. Директният маршрут и production заявка при недостъпна БД показват персонализирана 500 със статус 500 |

## Покритие

Coverlet измери **2/11 изпълними реда в `Services/` (18,18%)**. `HandoverPolicy` има 2/2 покрити реда, а `CollectionPointService` — 0/9 от unit тестовете. Покритието за целия инструментализиран модул, включително миграцията и Razor кода, е **2/2290 реда (около 0,09%)**. Отделните HTTP и SQL проверки са извън Coverlet. Локалните TRX и Cobertura файлове са в `TestResults/E1/`, а SQL изходът е в `D:\RePoint-Etap1\sql-verification.txt`. Финалният праг от 65% в бизнес логиката остава непостигнат.

## Корекции

- Identity регистрацията първоначално връщаше 500 заради липсващ `_LoginPartial` в стандартния layout. Identity страниците вече използват общия Razor layout.
- Грешки като 400 и 403 запазват HTTP статуса си. Персонализираните 404 и 500 се показват с правилните статуси.
- Валидацията на количество съответства на `decimal(12,3)`. Добавени са проверки за гранични стойности и сценарий с вече съществуващо потвърждение в началните тестове.
- Обновени са Identity и EF Core пакетите до 10.0.15. Предупрежденията NU1902 и NU1903 за транзитивния `System.Security.Cryptography.Pkcs` изчезнаха. [Указания на Microsoft](https://github.com/advisories/GHSA-846f-f764-76mx)
- Добавени са SQL ограничения, Identity роли и начална миграция. Docker портът е ограничен до localhost. CI е обновен да събира TRX и Cobertura.

## Финална повторна проверка

На 14.11.2026 г. повторно преминаха възстановяването на пакети, Release build с 0 грешки и 0 предупреждения и всички 12 unit теста. EF Core потвърди, че базата е актуална и моделът съвпада с миграцията. SQL проверката отново потвърди 16 таблици, ключовете, ограниченията и началните записи.

Клонът etap-1-verification е публикуван. CI на 4d508e4 премина успешно на GitHub runner, включително restore, build, unit тестове с покритие и качване на артефакта repoint-test-results. [GitHub Actions #1](https://github.com/CnapKy/RePoint/actions/runs/43531679673)

От текущите публични Markdown файлове са премахнати имената, класът и училището. Лицензът запазва оригиналното авторско означение, а предишните Git ревизии остават в историята. Документацията за споделяне, снимките на заданието, тестовите артефакти и локалните настройки остават извън Git.

## Оставащи действия

- Преглед на Pull Request, споделяне на DOCX и потвърждаване на достъпа на консултанта. Работният DOCX описва състоянието преди локалната проверка и следва да се сверява с този отчет.
- За незадължителния Docker вариант освободи дисково място, стартирай `wsl --install` в администраторски PowerShell, рестартирай, инсталирай и стартирай Docker Desktop с WSL 2 и провери `docker version` и `docker compose version`. [WSL](https://learn.microsoft.com/en-us/windows/wsl/install), [Docker Desktop](https://docs.docker.com/desktop/setup/install/windows-install/)
- Преди публично хостване обнови .NET 10 runtime до актуалната корекция и завърши потвърждаването на имейл, ролите и правата. Операторско потвърждение, отчети, карта, филтър по работно време, управление, сигнали, известия, публичен URL и демо акаунти остават за следващи етапи. [Актуален .NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Предстоят интеграционни тестове за завършените процеси, проверки за SQL Injection и XSS, по-широко тестване на устройства и браузъри и измерване на покритието спрямо финалния праг. Подробности има в [матрицата](docs/requirements-traceability.md).
