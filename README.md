[README (1).md](https://github.com/user-attachments/files/26868476/README.1.md)
# ProcessManagerUltimate

WPF-приложение для управления процессами операционной системы Windows с возможностью изменения приоритетов, настройки CPU Affinity, просмотра потоков и дерева процессов.

---

## Возможности

- Отображение списка всех запущенных процессов с характеристиками: PID, имя, приоритет, память, потоки, время CPU
- Изменение приоритета процесса (Низкий, Ниже среднего, Средний, Выше среднего, Высокий, Реального времени)
- Управление CPU Affinity — привязка процесса к конкретным ядрам через чекбоксы или строку ввода
- Просмотр потоков выбранного процесса
- Построение дерева процессов (родитель–потомок) через WMI
- Визуализация загрузки CPU в виде линейного графика (LiveCharts)
- Топ-10 процессов по потреблению памяти
- Массовые операции над несколькими выбранными процессами
- Автообновление данных каждую секунду

---

## Технологии

| Технология | Описание |
|---|---|
| C# / .NET Framework 4.8 | Язык и платформа |
| WPF | Пользовательский интерфейс |
| MVVM | Архитектурный паттерн |
| System.Diagnostics.Process | Работа с процессами |
| WMI (Win32_Process) | Получение родительских PID |
| LiveCharts | График загрузки CPU |
| NUnit 3 | Unit-тестирование |
| Coverlet + ReportGenerator | Покрытие кода |

---

## Структура проекта

```
ProcessManagerUltimate/
├── Converters/
│   ├── BytesToMBConverter.cs        # Байты → МБ
│   ├── CoreRangeConverter.cs        # Диапазон ядер для заголовка
│   ├── PriorityToBrushConverter.cs  # Цвет строки по приоритету
│   └── PriorityToRussianConverter.cs# Приоритет на русском языке
├── Models/
│   ├── ProcessInfo.cs               # Модель процесса
│   ├── ThreadInfo.cs                # Модель потока
│   ├── ProcessNode.cs               # Узел дерева процессов
│   └── PriorityClassItem.cs         # Элемент списка приоритетов
├── Services/
│   ├── ProcessService.cs            # Работа с процессами через API
│   ├── CpuMonitorService.cs         # Мониторинг загрузки ядер
│   └── ProcessTreeService.cs        # Построение дерева процессов
├── ViewModels/
│   ├── BaseViewModel.cs             # Базовый класс с INotifyPropertyChanged
│   └── MainViewModel.cs             # Логика главного окна
└── Views/
    └── MainWindow.xaml              # Главное окно приложения
```

---

## Запуск

### Требования

- Windows 10 / 11
- .NET Framework 4.8
- Visual Studio 2022

### Установка и запуск

1. Клонируй репозиторий:
```bash
git clone https://github.com/ZPatriot-cyber/ProcessManagerUltimate.git
```
2. Открой `ProcessManagerUltimate.sln` в Visual Studio
3. Нажми **F5** или **Пуск**

> ⚠️ Для изменения приоритетов системных процессов может потребоваться запуск от имени администратора.

---

## Тестирование

Тесты находятся в проекте `TestProject1` и покрывают конвертеры и модели данных.

### Запуск тестов в Visual Studio

```
Тест → Запустить все тесты
```

Или через терминал:
```bash
dotnet test
```

### Покрытие кода

```bash
# Сбор покрытия
dotnet test --collect:"XPlat Code Coverage"

# Генерация HTML-отчёта
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:Html

# Открытие отчёта
start coverage\index.html
```

---

## Архитектура

Приложение построено по паттерну **MVVM**:

- **Model** — `ProcessInfo`, `ThreadInfo`, `ProcessNode` — хранят данные
- **ViewModel** — `MainViewModel` — содержит всю логику, команды и свойства для биндинга
- **View** — `MainWindow.xaml` — только разметка, без логики
- **Services** — `ProcessService`, `CpuMonitorService`, `ProcessTreeService` — инкапсулируют работу с ОС

---

## Автор

Соловьёв М.С., группа Б.ИВТ.ПРОМ.23.01  
ТвГТУ, кафедра информационных систем, 2026
