using System;
using System.Collections;
using System.Collections.Generic;
     

namespace dungeon_crawl
{
    class Program
    {
        const int WIDTH = 10;
        const int HEIGHT = 7;

        static void Main(string[] args)
        {
            ArrayList bad_stuff = new ArrayList();
            ArrayList stuff_on_board = new ArrayList();
            ArrayList no_collision = new ArrayList();

            Board board = new Board();

            User user = new User(0, 0);
            Trap trap1 = new Trap(2, 6);
            Trap trap2 = new Trap(4, 4);
            Trap trap3 = new Trap(5, 6);
            Enemy enemy1 = new Enemy(3, 3);
            Enemy enemy2 = new Enemy(4, 4);
            Enemy enemy3 = new Enemy(5, 2);
            Enemy enemy4 = new Enemy(4, 6);
            Treasure treasure = new Treasure(6, 9);
            int win = 0;

            Console.WriteLine(no_collision);

            bad_stuff.Add(trap1);
            bad_stuff.Add(trap2);
            bad_stuff.Add(trap3);
            bad_stuff.Add(enemy1);
            bad_stuff.Add(enemy2);

            stuff_on_board.Add(user);
            stuff_on_board.Add(treasure);
            stuff_on_board.Add(trap1);
            stuff_on_board.Add(trap2);
            stuff_on_board.Add(trap3);
            stuff_on_board.Add(enemy1);
            stuff_on_board.Add(enemy2);
            stuff_on_board.Add(enemy3);
            stuff_on_board.Add(enemy4);

            no_collision.Add(treasure);
            no_collision.Add(trap1);
            no_collision.Add(trap2);
            no_collision.Add(trap3);
            no_collision.Add(enemy1);
            no_collision.Add(enemy2);
            no_collision.Add(enemy3);
            no_collision.Add(enemy4);

            do
            {
                board.clear();
                board.update(stuff_on_board);
                board.display();
                user.move(no_collision);
                enemy1.move(no_collision);
                enemy2.move(no_collision);
                enemy3.move(no_collision);
                enemy4.move(no_collision);
                for (int i = 0; i < bad_stuff.Count; i++)
                {
                    Character character = (Character)bad_stuff[i];
                    if (user.get_column() == character.get_column() && user.get_row() == character.get_row())
                    {
                        win = -1; //lost
                    }
                }
                if (user.get_column() == treasure.get_column() && user.get_row() == treasure.get_row())
                {
                    win = 1;
                }
            } while (win == 0);

            if (win == -1) 
            {
                Console.WriteLine("Good luck next time.. :)");
            } else
            {
                Console.WriteLine("Yes, you won! :D");    
            }
        }

        public struct Position
        {
            public int row, column;

            public Position(int num1, int num2) 
            {
                row = num1;
                column = num2;
            }
        }

        class Board
        {
            private char[,] board = new char[HEIGHT, WIDTH];

            public Board()
            {
                this.clear();
            }

            //Current problem: Enemeies and traps may cluster at the same place.
            public void update(ArrayList stuff_on_board) 
            {
                int count = stuff_on_board.Count;
                int row, column;
                Character character = new Character();

                for (int i = 0; i < count; i++)
                {
                    character = (Character)stuff_on_board[i];
                    row = character.get_row();
                    column = character.get_column();
                    this.board[row, column] = character.get_symbol();
                }
            }
                            
            public void clear()
            {
                //initialise a new board 
                for (int i = 0; i < HEIGHT; i++)
                {
                    for (int j = 0; j < WIDTH; j++)
                    {
                        this.board[i, j] = '.';
                    }
                }
            }

            public void display()
            {
                //content of a line
                string line;

                Console.WriteLine();
                for (int i = 0; i < HEIGHT; i++)
                {
                    line = "";
                    for (int j = 0; j < WIDTH; j++)
                    {
                        line = line += Convert.ToString(this.board[i, j]);
                    }
                    Console.WriteLine(line);
                }
            }
        }

        public class Character 
        {
            protected Position position = new Position();
            protected char symbol; 

            public Character() {}

            public Character(int row, int column)
            {
                this.position.row = row;
                this.position.column = column;
            }

            public Position get_position()
            {
                return this.position;
            }

            public int get_row() 
            {
                return this.position.row;
            }

            public int get_column()
            {
                return this.position.column;
            }

            public char get_symbol() 
            {
                return this.symbol;
            }

            public virtual void move(ArrayList stuff_on_board) {}

            //This does not currently check if two enemies are in the same position
            protected bool check_valid_move(char direction, Position position)
            {
                bool valid_move = true;

                switch (direction) 
                {
                    case 'u':
                        if (position.row == 0) valid_move = false;
                        break;
                    case 'd':
                        if (position.row == HEIGHT - 1) valid_move = false;
                        break; 
                    case 'l':
                        if (position.column == 0) valid_move = false;
                        break;
                    case 'r':
                        if (position.column == WIDTH - 1) valid_move = false;
                        break;
                }

                if (valid_move == false)
                {
                    Console.WriteLine("The move is not valid. Please enter again.");
                }

                return valid_move;
            }

            //Update the position after a move has been made
            protected void update_position(char direction)
            {
                switch (direction)
                {
                    case 'u':
                        this.position.row--;
                        break;
                    case 'd':
                        this.position.row++;
                        break;
                    case 'r':
                        this.position.column++;
                        break;
                    case 'l':
                        this.position.column--;
                        break;
                }            
            }
        }

        class User : Character
        {
            public User(int row, int column) : base(row, column)
            {
                this.symbol = 'G';
            }

            public override void move(ArrayList stuff_on_board)
            {
                char direction;
                bool valid_move = true;

                do
                {
                    direction = which_move();
                    valid_move = this.check_valid_move(direction, position);
                } while (valid_move == false);
                base.update_position(direction);
            }

            private char which_move() 
            {
                char direction;
                do
                    {
                        // this can be modified to response upon key clicks
                        Console.WriteLine("Up (u), Down (d), Left (l) or Right (r)?");
                        direction = Convert.ToChar(Console.ReadLine());
                    } while (!(direction == 'u' || direction == 'd' || direction == 'l' || direction == 'r'));

                return direction;
            }

        }

        class Enemy : Character 
        {
            public Enemy(int row, int column) : base(row, column)
            {
                this.symbol = 'E'; 
            }

            public override void move(ArrayList stuff_on_board)
            {
                Random rnd = new Random();
                Character character = new Character();

                int dice;
                char direction = ' ';
                bool valid_move;
                bool collision;
                int row, column;
                int new_row, new_column;

                do
                {
                    collision = false;
                    do
                    {
                        dice = rnd.Next(0, 4);
                        switch (dice)
                        {
                            case 0:
                                direction = 'u';
                                break;
                            case 1:
                                direction = 'd';
                                break;
                            case 2:
                                direction = 'l';
                                break;
                            case 3:
                                direction = 'r';
                                break;
                        }
                        valid_move = base.check_valid_move(direction, this.position);
                    } while (valid_move == false);

                    //old positions
                    row = this.position.row;
                    column = this.position.column;
                    for (int i = 0; i < stuff_on_board.Count; i++)
                    {
                        character = (Character)stuff_on_board[i];
                        base.update_position(direction);

                        //would it cause problems if assign the attribute to another object attribute instead?
                        new_row = this.position.row;
                        new_column = this.position.column;
                        //setting back to old values 
                        this.position.row = row;
                        this.position.column = column;

                        if (new_row == character.get_row() && new_column == character.get_column())
                        {
                            collision = true;
                           // Console.WriteLine("Collision row column " + character.get_row() + " " + character.get_column());
                           // Console.WriteLine();
                            break;
                        }
                    }
                } while (collision == true);
                base.update_position(direction);
            }

        }

        class Treasure : Character
        {
            public Treasure (int row, int column) : base(row, column) 
            {
                this.symbol = 'X';
            }
        }

        class Trap : Character 
        {
            public Trap (int row, int column) : base(row, column) 
            {
                this.symbol = 'T';
            }
        }
    }
}
