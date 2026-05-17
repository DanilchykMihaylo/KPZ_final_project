# Гра "Шашки"

Сучасна реалізація класичної гри в шашки на WPF + C#  
з дотриманням принципів чистої архітектури, патернів проєктування  
та сучасних практик розробки.

---

## Функціонал програми

1. Основна дошка з фігурами та реалізація всіх правил гри в шашки
2. Таймер та рахунок, які дозволяють слідкувати за ходом гри
3. Можливість зберегти та завантажити партію (JSON)
4. Можливість змінювати тему
5. Перемога білих/чорних, нічия та рахунок перемог
6. Можливість почати нову гру

---

## Принципи програмування

### SRP (Single Responsibility Principle)
- `Board` — лише дані
- `GameService` — логіка
- `GameViewModel` — UI-стан

### OCP (Open/Closed Principle)
- `IMoveValidator`
- `IMoveGenerator`
- Розширення без зміни існуючого коду

### DIP (Dependency Inversion Principle)
- `GameService` отримує `IMoveGenerator`
- `GameViewModel` отримує `IGameService`

### ISP (Interface Segregation Principle)
- Окремі інтерфейси:
  - `IMoveValidator`
  - `IMoveGenerator`

### DRY (Don't Repeat Yourself)
- `BaseViewModel` з `SetField`
- `GetDirections` централізує напрямки ходів

---

## Патерни

### MVVM
`GameViewModel` ↔ `GameWindow.xaml`  
через `INotifyPropertyChanged` та `ICommand`

### Command
- `RelayCommand`
- `RelayCommand<T>`
- `CellClickCommand`
- `NewGameCommand`

### Strategy
`IMoveValidator` — алгоритм валідації можна замінити  
без зміни генератора

---

## Техніки

- **Immutability** — `Position` з readonly-властивостями, `sealed class`
- **Pattern Matching** — switch-вирази в `UpdateGameState`, `GetDirections`
- **Null Safety** — `nullable enable`, `?.`, `is null`, `is not null`
- **LINQ** — `GetAllPieces`, `GetAvailableMoves`, `GetPiecesByColor`
- **Data Binding** — WPF `{Binding}`, `ObservableCollection`, `DataTrigger`
