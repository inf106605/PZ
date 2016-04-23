using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GongSolutions.Wpf.DragDrop;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SkyCrab.Classes.Game
{

    class ScrabbleRack : IDropTarget
    {

       static public ObservableCollection<ScrabbleRackTiles> RackTiles { get; set; }
        
        public ScrabbleRack()
        {
            RackTiles = new ObservableCollection<ScrabbleRackTiles>();

            RackTiles.Add(new ScrabbleRackTiles(1, "A", 1));
            RackTiles.Add(new ScrabbleRackTiles(2, "Ź", 9));
            RackTiles.Add(new ScrabbleRackTiles(3, "D", 2));
            RackTiles.Add(new ScrabbleRackTiles(4, "H", 3));
            RackTiles.Add(new ScrabbleRackTiles(5, "I", 2));
            RackTiles.Add(new ScrabbleRackTiles(6, "G", 4));
            RackTiles.Add(new ScrabbleRackTiles(7, "M", 5));

        }


        public void DragOver(DropInfo dropInfo)
        {
            if (dropInfo.Data is ScrabbleRackTiles)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
        }

        public void Drop(DropInfo dropInfo)
        {
            ScrabbleRackTiles msp = (ScrabbleRackTiles)dropInfo.Data;
            ((IList)dropInfo.DragInfo.SourceCollection).Remove(msp);
        }
    }
}
