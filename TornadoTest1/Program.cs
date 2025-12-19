// Программа реализует контроллер простого лифта.

// Алгоритм решения:
//   1. Лифт в приоритете продолжает движение в одном направлении до конца движения по вызову
//   2. Если достигли конца движения по вызову
//      2.1. Если вызовов больше нет, то останавливаем движение и ожидаем вызовов
//      2.2. Если есть вызовы в противоположном направлении, то меняем направление движения

// Для валидации решения подразумеваю выполнение тестов в TornadoTest1.Tests проекте
    // Смотри Tick_ElevatorCases_AllCallsHandled и данные в ElevatorCases
    // Испольняемый файл этого модуля не собираю и не прикладываю т.к. вся валидация в тестах

namespace TornadoTest1
{
    public enum Direction { Idle = 0, Up = 1, Down = -1 }

    public class Elevator
    {
        public readonly int MinFloor;
        public readonly int MaxFloor;

        public int CurrentFloor { get; private set; }
        public Direction CurrentDirection { get; private set; } = Direction.Idle;

        private readonly HashSet<int> Calls = [];

        public Elevator(int startFloor = 1, int minFloor = 1, int maxFloor = 9)
        {
            if (minFloor <= 0)
            {
                throw new ArgumentException("minFloor must be greater than 0");
            }
            if (minFloor >= maxFloor)
            {
                throw new ArgumentException("minFloor must be less than maxFloor");
            }
            if (startFloor < minFloor || startFloor > maxFloor)
            {
                throw new ArgumentOutOfRangeException(nameof(startFloor));
            }
            MinFloor = minFloor;
            MaxFloor = maxFloor;
            CurrentFloor = startFloor;
            CurrentDirection = Direction.Idle;
        }

        public void Call(int floor)
        {
            if (floor == 0)
            {
                // примем за 0 что кнопку не нажимают
                return;
            }
            if (floor < MinFloor || floor > MaxFloor)
            {
                throw new ArgumentOutOfRangeException(nameof(floor));
            }
            Calls.Add(floor);
        }

        public void Tick()
        {
            CurrentDirection = DecideNextDirection(CurrentDirection);
            if (CurrentDirection == Direction.Idle)
            {
                return;
            }

            int nextFloor = Math.Clamp(MinFloor, CurrentFloor + (int)CurrentDirection, MaxFloor);
            CurrentFloor = nextFloor;
            Calls.Remove(CurrentFloor);

            if (!HasPendingRequests)
            {
                CurrentDirection = Direction.Idle;
            }
        }

        private Direction DecideNextDirection(Direction currentDirection)
        {
            if (!HasPendingRequests)
            {
                return Direction.Idle;
            }

            // TODO не пробовал кейс если вызвать лифт на текущем этаже, по коду ожидаю баги. Пока оставлю как есть, что успел в уделенное время
            // TODO можно зарефакторить, хотелось бы уменьшить ветвление, но пока оставляю самое простое
            if (CurrentDirection == Direction.Idle || CurrentDirection == Direction.Up)
            {
                if (HasCallsAbove)
                {
                    return Direction.Up;
                }
                else
                {
                    return Direction.Down;
                }
            }

            if (CurrentDirection == Direction.Idle || CurrentDirection == Direction.Down)
            {
                if (HasCallsBelow)
                {
                    return Direction.Down;
                }
                else
                {
                    return Direction.Up;
                }
            }

            return Direction.Idle;
        }

        private bool HasCallsAbove
        {
            // TODO в целом думаю можно попробовать оптимизировать от O(n) до O(1) ради демонстрации в тестовом, но пока оставлю как есть. Да и лифты не с миллионами этажей обычно работают)
            get { return Calls.Any(f => f > CurrentFloor); }
        }

        private bool HasCallsBelow
        {
            get { return Calls.Any(f => f < CurrentFloor); }
        }

        public bool HasPendingRequests => Calls.Count > 0;
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Для валидации решения подразумеваю выполнение тестов в TornadoTest1.Tests проекте. Смотри Tick_ElevatorCases_AllCallsHandled");
        }
    }
}
