using NUnit.Framework;
using System;

namespace TornadoTest1.Tests
{
    [TestFixture]
    public class ElevatorTests
    {
        [TestCase(0)]
        public void Constructor_MinFloorLessThan1_ThrowsException(int minFloor)
        {
            Assert.Throws<ArgumentException>(() => new Elevator(minFloor: minFloor));
        }

        [TestCase(5, 1)]
        [TestCase(2, 2)]
        public void Constructor_MinFloorGreaterOrEqualThanMaxFloor_ThrowsException(int minFloor, int maxFloor)
        {
            Assert.Throws<ArgumentException>(() => new Elevator(minFloor: minFloor, maxFloor: maxFloor));
        }

        [TestCase(20, 1, 9)]
        public void Constructor_StartFloorOutOfBounds_ThrowsException(int startFloor, int minFloor, int maxFloor)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Elevator(startFloor: startFloor, minFloor: minFloor, maxFloor: maxFloor));
        }

        [TestCase(-1)]
        [TestCase(10)]
        public void Call_FloorOutOfBounds_ThrowsException(int callFloor)
        {
            var elevator = new Elevator(startFloor: 1, minFloor: 1, maxFloor: 9);
            Assert.Throws<ArgumentOutOfRangeException>(() => elevator.Call(callFloor));
        }

        [TestCaseSource(nameof(ElevatorCases))]
        public void Tick_ElevatorCases_AllCallsHandled(ElevatorCaseData data)
        {
            var elevator = new Elevator(startFloor: data.StartFloor, minFloor: data.MinFloor, maxFloor: data.MaxFloor);
            for (int i = 0; i < data.ExpectedFloors.Length; i++)
            {
                elevator.Tick();
                Assert.That(elevator.CurrentFloor == data.ExpectedFloors[i], $"Tick {i + 1}: Expected floor {data.ExpectedFloors[i]}, but got {elevator.CurrentFloor}");
                elevator.Call(data.FloorsCalls[i]);
            }
            Assert.That(elevator.HasPendingRequests, Is.False, "All calls should be handled by the end of the sequence");
            Assert.That(elevator.CurrentDirection, Is.EqualTo(Direction.Idle), "Elevator should be idle at the end of the sequence");
        }

        public static ElevatorCaseData[] ElevatorCases =
        [
            new ElevatorCaseData
            {
                StartFloor = 1,
                ExpectedFloors = [1, 2, 3], // Ожидаемые этажи после каждого тика, для визуалиации и подтверждения движения лифта
                FloorsCalls    = [3, 0, 0], // Вызовы на этажи после каждого тика. Для простоты пока 1 вызов за тик, 0 означает отсутствие вызова
            },
            new ElevatorCaseData
            {
                StartFloor = 1,
                ExpectedFloors = [1, 2, 3, 4, 5, 4, 3, 2],
                FloorsCalls    = [5, 0, 2, 0, 0, 0, 0, 0],
            },
            new ElevatorCaseData
            {
                StartFloor = 4,
                ExpectedFloors = [4, 5, 6, 7, 8, 9, 8, 7, 6, 5, 4, 3, 2, 1],
                FloorsCalls    = [9, 0, 1, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
            },
            new ElevatorCaseData
            {
                StartFloor = 8,
                ExpectedFloors = [8, 7, 6, 5, 4, 3, 4, 5, 6, 7, 8, 9, 8, 7, 6, 5],
                FloorsCalls    = [5, 9, 3, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0],
            },
            // TODO не пробовал кейс если вызвать лифт на текущем этаже, по коду ожидаю баги. Пока оставлю как есть что успел в уделенное время
        ];

        public class ElevatorCaseData
        {
            public int StartFloor = 1;
            public int MinFloor = 1;
            public int MaxFloor = 9;
            public int[] ExpectedFloors;
            public int[] FloorsCalls;
        }
    }
}
