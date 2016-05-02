using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GongSolutions.Wpf.DragDrop;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using SkyCrab.Common_classes.Games.Tiles;

namespace SkyCrab.Classes.Game
{

    class ScrabbleRack : IDropTarget
    {

       private ObservableCollection<ScrabbleRackTiles> rackTiles;
       
       public ObservableCollection<ScrabbleRackTiles> RackTiles
       {
           get
           {
               return rackTiles;
           }
       }

        public void RemoveTile(int id)
        {
            rackTiles.Remove(rackTiles.Where(temp => temp.Id == id).Single());
        }
        
        public ScrabbleRackTiles SearchIdTile(ScrabbleRackTiles scrabbleRackTile)
        {
            return rackTiles.Where(temp => scrabbleRackTile.Id == temp.id).Single();
        }


        public ScrabbleRack()
        {
            rackTiles = new ObservableCollection<ScrabbleRackTiles>();
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
