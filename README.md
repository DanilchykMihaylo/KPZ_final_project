Гра "Шашки"

Сучасна реалізація класичної гри в шашки на WPF + C# з дотриманням принципів чистої архітектури, патернів проєктування та сучасних практик розробки.

Функціонал програми:
1)основна дошка з фігурами та пророботка всіх правил гри в шашки
2)таймер та рахунок які дозволяють слідкувати за ходом гри
3)можливість зберегти та загрузити партію (зроблено за допомогою json)
4)можливість міняти тему
5)перемога білих/чорних та нічья а також рахунок перемог
6)Можливість запустити нову гру

Принципи програмування:
SRP (Single Responsibility) Board — лише дані; GameService — логіка; GameViewModel — UI-стан
OCP (Open/Closed) IMoveValidator, IMoveGenerator — розширення без зміни коду
DIP (Dependency Inversion) GameService отримує IMoveGenerator; GameViewModel отримує IGameService
ISP (Interface Segregation) Окремі інтерфейси IMoveValidator і IMoveGenerator замість одного великого
DRY (Don't Repeat Yourself) BaseViewModel з SetField; GetDirections централізує напрямки ходів

Патерни :
MVVM GameViewModel ↔ GameWindow.xaml через INotifyPropertyChanged та ICommand
Command RelayCommand / RelayCommand<T> для CellClickCommand, NewGameCommand
Strategy IMoveValidator — алгоритм валідації можна замінити без зміни генератора

Техніки: 
Immutability Position — readonly властивості, sealed клас
Pattern Matching switch вирази в UpdateGameState, GetDirections
Null Safety Nullable enable, оператори ?., is not null, is null
LINQ GetAllPieces, GetAvailableMoves, GetPiecesByColor
Data Binding WPF {Binding} з ObservableCollection, DataTrigger для стилів
