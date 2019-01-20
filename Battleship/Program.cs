using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    public class Coordinates
    {
        public int XCoordinate;
        public int YCoordinate;
        public bool HasHit;
    }

    public class Ship
    {
        public List<Coordinates> coordinates;
        public bool HasSunk()
        {
            return (coordinates.Count() == coordinates.Where(c => c.HasHit == true).Count());
        }
    }

    class Program
    {
        static int boardSize = 10;

        static void Main(string[] args)
        {         
            List<Ship> ships = new List<Ship>();

            Console.WriteLine("Welcome to BattleShip State Tracker");
            Console.WriteLine("Please add Ships to the Game Board of " + boardSize + " by " + boardSize);
            Console.WriteLine("Enter the number of ships you would like to add:");

            int noOfShips;
            string response = Console.ReadLine();
            while (!Int32.TryParse(response, out noOfShips))
            {
                Console.WriteLine("Please enter an integer value, try again.");
                response = Console.ReadLine();
            }

            AddShipsConsoleMessages(noOfShips, ships);
            HasHitShipConsoleMessages(ships);
            Console.ReadLine();           
        }

        static void AddShipsConsoleMessages(int noOfShips, List<Ship> ships)
        {
            int X, Y, shipSize;
            string direction, response;

            int shipCounter = 0;

            while (shipCounter < noOfShips)
            {
                Console.WriteLine("Add Ship Details");

                // Enter X Coordinate
                Console.WriteLine("Enter X Coordinate:");
                response = Console.ReadLine();
                while (!Int32.TryParse(response, out X))
                {
                    Console.WriteLine("Please enter integer value, try again.");
                    response = Console.ReadLine();
                }

                // Enter Y Coordinate
                Console.WriteLine("Enter Y Coordinate:");
                response = Console.ReadLine();
                while (!Int32.TryParse(response, out Y))
                {
                    Console.WriteLine("Please enter integer value, try again.");
                    response = Console.ReadLine();
                }

                // Enter Ship Size 
                Console.WriteLine("Enter Ship Size:");
                response = Console.ReadLine();
                while (!Int32.TryParse(response, out shipSize))
                {
                    Console.WriteLine("Please enter integer value, try again.");
                    response = Console.ReadLine();
                }

                // Enter Direction
                Console.WriteLine("Enter Ship Direction - H (Horizontal - Left to Right) or V (Vertical - Bottom to Top):");
                response = Console.ReadLine();
                while (response.ToLower().Trim() != "h" && response.ToLower().Trim() != "v")
                {
                    Console.WriteLine("Please enter either H or V for direction Horizontal or Vertical.");
                    response = Console.ReadLine();
                }
                direction = response;

                shipCounter = AddShips(X, Y, shipSize, direction, ships, shipCounter);
            }

            Console.WriteLine("All Ships are added.");
        }

        static int AddShips(int X, int Y, int shipSize, string direction, List<Ship> ships, int shipCounter)
        {            
           
            if (AreCoordinatesValid(X, Y, shipSize, direction))
            {
                Ship newShip = new Ship();
                newShip.coordinates = SetShipCoordinates(X, Y, shipSize, direction);

                if (DoesShipOverlap(newShip, ships) == true)
                {
                    Console.WriteLine("Ship overlaps with another Ship, please re-enter Ship details");
                }
                else
                {
                    ships.Add(newShip);
                    shipCounter++;
                    Console.WriteLine("Ship Added");
                }
            }
            else
            {
                Console.WriteLine("Invalid entry, please try again");
            }

            return shipCounter;
        }

        static void HasHitShipConsoleMessages(List<Ship> ships)
        {
            int XAttack, YAttack;
            string response;

            while (HasPlayerLost(ships) == false)
            {                
                // Enter X Coordinate
                Console.WriteLine("Enter X Coordinate of attack:");
                response = Console.ReadLine();
                while (!Int32.TryParse(response, out XAttack))
                {
                    Console.WriteLine("Please enter integer value, try again.");
                    response = Console.ReadLine();
                }

                // Enter Y Coordinate
                Console.WriteLine("Enter Y Coordinate of attack:");
                response = Console.ReadLine();
                while (!Int32.TryParse(response, out YAttack))
                {
                    Console.WriteLine("Please enter integer value, try again.");
                    response = Console.ReadLine();
                }

                if (HasHit(XAttack, YAttack, ships) == false)
                {
                    Console.WriteLine("You have missed!");
                }
            }
        }

        static bool HasHit(int XAttack, int YAttack, List<Ship> ships)
        {
            bool hasHitShip = false;
            foreach (Ship s in ships)
            {
                Coordinates shipCoordinate = s.coordinates.Where(c => c.XCoordinate == XAttack && c.YCoordinate == YAttack).FirstOrDefault();

                if (shipCoordinate != null)
                {
                    if (shipCoordinate.HasHit == true)
                    {
                        Console.WriteLine("This target is hit already.");
                        hasHitShip = true;
                    }
                    else
                    {
                        shipCoordinate.HasHit = true;

                        hasHitShip = true;
                        Console.WriteLine("Ship is HIT");

                        if (s.HasSunk())
                        {
                            Console.WriteLine("Ship has Sunk!");

                            if (HasPlayerLost(ships))
                            {
                                Console.WriteLine("You have Lost the Game!");                                
                            }
                        }
                        break;
                    }
                }

            }

            return hasHitShip;
        }
        
        static bool AreCoordinatesValid(int X, int Y, int shipSize, string direction)
        {
            int coordinatePrimary = direction.ToLower() == "h" ? X : Y;
            int coordinateSecondary = direction.ToLower() == "h" ? Y : X;

            if (coordinatePrimary > boardSize - shipSize - 1)
            {
                return false;
            }
            else if (coordinateSecondary >= boardSize)
            {
                return false;
            }
            return true;
        }

        static bool DoesShipOverlap(Ship ship, List<Ship> ships)
        {
            List<Coordinates> shipCoordiatesOnBoard = ships.SelectMany(s => s.coordinates).ToList();

            var checkIfShipOverlap = from scb in shipCoordiatesOnBoard
                                     join sc in ship.coordinates on
                                             new { scb.XCoordinate, scb.YCoordinate } equals new { sc.XCoordinate, sc.YCoordinate }
                                     select new { scb.XCoordinate, scb.YCoordinate };

            if (checkIfShipOverlap != null && checkIfShipOverlap.Count() > 0)
                return true;

            else
                return false;
        }       

        static List<Coordinates> SetShipCoordinates(int X, int Y, int shipSize, string direction)
        {
            List<Coordinates> shipCoordinates = new List<Coordinates>();

            Coordinates firstCoordinate = new Coordinates();
            firstCoordinate.XCoordinate = X;
            firstCoordinate.YCoordinate = Y;

            shipCoordinates.Add(firstCoordinate);
            int coordinateIncrement;

            if (direction == "h")
            {
                coordinateIncrement = X;

                while (coordinateIncrement < firstCoordinate.XCoordinate + shipSize)
                {
                    coordinateIncrement = coordinateIncrement + 1;
                    Coordinates sc = new Coordinates();

                    sc.XCoordinate = coordinateIncrement;
                    sc.YCoordinate = Y;

                    shipCoordinates.Add(sc);
                }
            }
            else
            {
                coordinateIncrement = Y;

                while (coordinateIncrement < firstCoordinate.YCoordinate + shipSize)
                {
                    coordinateIncrement = coordinateIncrement + 1;
                    Coordinates sc = new Coordinates();

                    sc.XCoordinate = X;
                    sc.YCoordinate = coordinateIncrement;

                    shipCoordinates.Add(sc);
                }
            }

            return shipCoordinates;
        }

        static bool HasPlayerLost(List<Ship> ships)
        {
            foreach(Ship s in ships)
            {
                bool a = s.HasSunk(); 
            }
            return (ships.Count() == ships.Where(s => s.HasSunk() == true).Count()) ? true : false;
        }
    }
}
