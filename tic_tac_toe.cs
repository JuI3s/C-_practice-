using System;
using System.Collections;

namespace tic
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board();
            Game game = new Game();

            board.clear_board();
            board.draw_board();
            game.play_game(board);
        }

        public static int get_row() {
            int row;

            do
            {
                Console.WriteLine("Enter the row number for your move (between 1 and 3): ");
                row = Convert.ToInt16(Console.ReadLine());
            } while (row < 1 || row > 3);
            return row; 
        }

        public static int get_column()
        {
            int column;
            do
            {
                Console.WriteLine("Enter the column number for your move: ");
                column = Convert.ToInt16(Console.ReadLine());
            } while (column < 1 || 

                     column > 3);
            return column;
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
            const int DIM = 3;
            private char[,] board = new char[DIM, DIM];
            private ArrayList available_grids = new ArrayList();

            public ArrayList get_available_grids() {
                return available_grids;
            }

            public char[,] get_board() {
                return board;
            }

            public void clear_board()
            {
                for (int i = 0; i < DIM; i++)
                {
                    for (int j = 0; j < DIM; j++)
                    {
                        Position position = new Position(i, j);
                        board[i, j] = ' ';
                        available_grids.Add(position);
                    }
                }
            }

            public void draw_board()
            {
                Console.WriteLine();
                Console.WriteLine("*******");
                for (int i = 0; i < DIM; i++)
                {
                    string write = "|";
                    for (int j = 0; j < DIM; j++)
                    {
                        write += board[i, j];
                        write += "|";
                    }
                    Console.WriteLine(write);
                }
                Console.WriteLine("*******");
            }

            public void update_move(char move, Position position) {
                int row, column;
                row = position.row;
                column = position.column;
                board[row, column] = move;
                available_grids.Remove(position);
            }
        }

        class Game {
            private string winner = "none";
            private bool cross_turn = true; //true if the next turn is cross
            Position safety_move = new Position();

            private void change_turn() {
                cross_turn = !cross_turn;
            }

            private void make_move(Board board) {
                //A player makes a move
                char move;
                Position position = new Position(); 

                if (cross_turn) {
                    move = 'X';
                    position = this.human_make_move(board);
                    Console.WriteLine("Player makes a move.");
                } else {
                    move = 'O';
                    position = this.computer_makes_move(board);
                    Console.WriteLine("Computer makes a move.");
                }
                board.update_move(move, position); 
                board.draw_board();
                this.change_turn();
            }

            private Position human_make_move(Board board) 
            {
                Position position = new Position();
                bool valid_move;

                //row and columns start at 0
                do
                {
                    position.row = get_row() - 1;
                    position.column = get_column() - 1;
                    valid_move = validate_move(position, board);
                    if (!valid_move)
                    {
                        Console.WriteLine("Place already taken. Please enter another position.");
                    }
                } while (valid_move == false);
                return position;
            }

            private bool validate_move(Position position, Board board)
            {
                ArrayList available_grids = board.get_available_grids();
                bool valid_move = true;

                if (!(available_grids.Contains(position))) {
                    valid_move = false;
                }
                return valid_move;
            }

            private Position computer_makes_move(Board Board) 
            {
                bool make_safety_move;
                Position position = new Position();

                make_safety_move = this.check_almost_win(Board);
                if (make_safety_move) {
                    position = this.safety_move;
                } else 
                {
                    position = this.computer_makes_random_move(Board);
                }
                return position;
            }

            private Position computer_makes_random_move(Board board) 
            {
                Random rnd = new Random();
                ArrayList available_grids = new ArrayList();
                int count;
                int index;
                Position position = new Position();

                available_grids = board.get_available_grids();
                count = available_grids.Count;
                index = rnd.Next(0, count);
                position = (Position)available_grids[index];
                return position;
            }

            private bool check_almost_win(Board Board) 
            {
                ArrayList available_grids = Board.get_available_grids();
                Position position = new Position();
                char[,] board = Board.get_board();
                bool make_safety_move = false;

                for (int i = 0; i < available_grids.Count; i++) 
                {
                    position = (Position)available_grids[i];
                    board[position.row, position.column] = 'X';
                    make_safety_move = this.check_win(Board);
                    if (make_safety_move == true) 
                    {
                        //reset the winner as the move is not made
                        this.winner = "none";
                        this.safety_move = position;
                        //reset the position on the board.
                        board[position.row, position.column] = ' ';
                        break;
                    }
                    board[position.row, position.column] = ' ';
                }
                return make_safety_move;
            }

            private bool check_win(Board Board) {
                //Check if the game is still ongoing and if finished who wins
                //Exhaustive search of all winning conditions 
                char[,] board = Board.get_board();
                for (int i = 0; i < 3; i++) { 
                    //check horizontal winning conditions
                    if (board[i,0] != ' ' && board[i,0] == board[i,1] && board[i, 1] == board[i,2]) {
                        winner = Convert.ToString(board[i, 0]);
                        return true; 
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    //check vertical winning conditions
                    if (board[0,i] != ' ' && board[0, i] == board[1, i] && board[1, i] == board[2, i])
                    {
                        winner = Convert.ToString(board[0, i]);
                        return true;
                    }
                }

                //check diagonal winning conditions
                if (board[1,1] != ' ' && board[0, 0] == board[1,1] && board[1,1] == board[2,2]) {
                    winner = Convert.ToString(board[1, 1]);
                    return true;
                } else if (board[1, 1] != ' ' && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0]){
                    winner = Convert.ToString(board[1, 1]);
                    return true; 
                }
                return false;
            }

            private bool check_finish(Board Board) {
                //check if a game is finished
                bool finish = false;
                ArrayList available_grids = Board.get_available_grids();
                if (available_grids.Count == 0)
                {
                    finish = true;
                }
                return finish;
            }

            private void display_winning_message() {
                switch (winner) {
                    case "none":
                        Console.WriteLine("This is a draw.");
                        break;
                    case "X":
                        Console.WriteLine("The player (X) wins");
                        break; 
                    case "O":
                        Console.WriteLine("The computer wins");
                        break;
                }
            }

            private void display_game_over_message() {
                if (this.winner == "none") {
                    Console.WriteLine("The game is a draw");
                } else if (this.winner == "X") {
                    Console.WriteLine("X wins");
                } else if (this.winner == "O") {
                    Console.WriteLine("O wins");
                }
            }

            public void play_game(Board board) {
                bool win;
                bool finish;
                do
                {
                    make_move(board);
                    win = check_win(board);
                    finish = check_finish(board);
                } while (win != true && finish != true);
                display_winning_message();
            }
        }
    }
}
