using SkyCrab.Common_classes.Games.Boards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SkyCrab.Classes.Game
{
    public class ChessBoard
    {
        public List<ChessSquare> Squares { get; private set; }

        public Command<ChessSquare> SquareClickCommand { get; private set; }

        
        public ChessBoard()
        {
            Squares = new List<ChessSquare>();

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Squares.Add(new ChessSquare(i,j));
                }
            }

            SquareClickCommand = new Command<ChessSquare>(OnSquareClick);
        }

        private void OnSquareClick(ChessSquare square)
        {
            System.Windows.MessageBox.Show("You clicked on Row: " + square.Row + " - Column: " + square.Column);
            
        }
    }

    public class ChessSquare
    {

        static StandardBoard board = new StandardBoard();
        private SquareType squareType;

        public ChessSquare(int Row, int Column)
        {
            this.Row = Row;
            this.Column = Column;
            this.squareType = board.GetQuareType(new PositionOnBoard(Row, Column));
        }


        public int Row { get; set; }

        public int Column { get; set; }

        public bool IsDarkBlue
        {
            get
            {
                return squareType == SquareType.letter3;
            }
        }

        public bool IsRed
        {
            get
            {
                return squareType == SquareType.word3;
            }
        }

        public bool IsOrange
        {
            get
            {
                return squareType == SquareType.word2 || squareType == SquareType.start;
            }
        }

        public bool IsBlue { get {

                return squareType == SquareType.letter2;

            } }

        public bool IsStart { get {

                return squareType == SquareType.word3;

            } }

        public bool IsGreen { get {

                return squareType == SquareType.normal;
            } }

    }

    public class Command<T> : ICommand
    {
        public Action<T> Action { get; set; }

        public void Execute(object parameter)
        {
            if (Action != null && parameter is T)
                Action((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;

        public Command(Action<T> action)
        {
            Action = action;
        }
    }
}
