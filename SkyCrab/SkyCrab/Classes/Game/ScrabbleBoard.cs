using SkyCrab.Common_classes.Games.Boards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SkyCrab.Classes.Game
{
    public class ScrabbleBoard
    {
        public List<ScrabbleSquare> Squares { get; private set; }

        public Command<ScrabbleSquare> SquareClickCommand { get; private set; }

        public ScrabbleBoard()
        {
            Squares = new List<ScrabbleSquare>();

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Squares.Add(new ScrabbleSquare(i,j));
                }
            }

            SquareClickCommand = new Command<ScrabbleSquare>(OnSquareClick);
        }

        private void OnSquareClick(ScrabbleSquare square)
        {
            System.Windows.MessageBox.Show("You clicked on Row: " + square.Row + " - Column: " + square.Column);
            
        }
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
