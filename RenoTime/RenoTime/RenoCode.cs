using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Enums.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;


namespace RenoTime
{
    public class RenoCode
    {
        
        private static HearthstoneTextBlock _info;
        private static List<Card> Dupes { get; set; }
        private static bool HasReno { get; set; }
        private static Entity Player { get { return Entities == null ? null : Entities.First(x => x.IsPlayer); } }
        
        private static Entity[] Entities
        {
            // Get the Game.Entities
            get
            {
                return Helper.DeepClone<Dictionary<int, Entity>>(
                    Hearthstone_Deck_Tracker.API.Core.Game.Entities).Values.ToArray<Entity>();
            }
        }

        public static void Load()
        {

            Logger.WriteLine("Load();","RenoTime");

            Dupes = new List<Card>();

            // A border to put around the text block
            Border blockBorder = new Border();
            blockBorder.BorderBrush = Brushes.Black;
            blockBorder.BorderThickness = new Thickness(1.0);
            blockBorder.Padding = new Thickness(8.0);

            // A text block using the HS font
            _info = new HearthstoneTextBlock();
            _info.Text = "";
            _info.FontSize = 18;
            _info.Fill = Brushes.Yellow;

            // Add the text block as a child of the border element
            blockBorder.Child = _info;

            // Get the HDT Overlay canvas object
            var canvas = Hearthstone_Deck_Tracker.API.Core.OverlayCanvas;
            // Get canvas centre
            var fromTop = canvas.Height / 2;
            var fromLeft = canvas.Width / 2;
            // Give the text block its position within the canvas, roughly in the center
            Canvas.SetTop(blockBorder, fromTop);
            Canvas.SetLeft(blockBorder, fromLeft);
            // Give the text block its position within the canvas
            
            
            // Add the text block and image to the canvas
            canvas.Children.Add(blockBorder);

   
            // Register methods to be called when GameEvents occur
            GameEvents.OnGameStart.Add(NewGame);
            GameEvents.OnGameEnd.Add(GameOver);
            GameEvents.OnInMenu.Add(GameOver);
            GameEvents.OnPlayerDraw.Add(CardDrew);
            GameEvents.OnTurnStart.Add(TurnStart);
            

            
            
        }


        public static void NewGame()
        {
            Logger.WriteLine("NewGame();", "RenoTime");

            var Reno = DeckList.Instance.ActiveDeck.Cards.FirstOrDefault(x => x.Name == "Reno Jackson");
            if(Reno == null)
            {
                _info.Visibility = Visibility.Hidden;
                HasReno = false;
            }
            else
            {
                Dupes = new List<Card>();
                _info.Visibility = Visibility.Visible;
                _info.Fill = Brushes.Yellow;
                HasReno = true;
                LoadDupes();
                CheckDupes();
            }           


        }

        public static void TurnStart(ActivePlayer player)
        {
            //Dupe Checks for Player
            if(player == ActivePlayer.Player)
            {
                Logger.WriteLine("Player Turn", "RenoTime");
                CheckDupes();
            }
        }

        public static void CardDrew(Card card)
        {
            if (HasReno == false) return;

            Logger.WriteLine("CardDrew(); " + card.Name, "RenoTime");
            var mulliganIsDone = Hearthstone_Deck_Tracker.API.Core.Game.IsMulliganDone;

            Logger.WriteLine("MulliganIsDone: " + mulliganIsDone.ToString(), "RenoTime");

            if (mulliganIsDone == false) return;

            CheckForDupe(card);
           

        }

        public static void CheckDupes()
        {
            int controller = Player.GetTag(GAME_TAG.CONTROLLER);
            if (HasReno == false) return;
                  
            foreach (var e in Entities)
            {
               //If The Card was Active some how...Dupe Check it
                                
                if (e.IsPlayer) continue; 
                if (e.IsOpponent) continue;
                if (e.GetTag(GAME_TAG.CONTROLLER) != Player.GetTag(GAME_TAG.CONTROLLER)) continue; //Skip Non-Player Entities

                if(e.IsInPlay || e.IsInGraveyard || e.IsInHand || e.IsSecret)
                {
                    CheckForDupe(e.Card);
                }
                           

            }

            Logger.WriteLine("All Entities Checked", "RenoTime");
            
        }

        public static void GameOver()
        {

            Logger.WriteLine("GameOver();", "RenoTime");
            _info.Visibility = Visibility.Hidden;
        }

        public static void LoadDupes()
        {
            if (HasReno == false) return;
            Logger.WriteLine("LoadDupes();", "RenoTime");

            Dupes.Clear();
            var deck = DeckList.Instance.ActiveDeck;

            _info.Text = string.Empty;

            foreach (var c in deck.Cards.Where(x=>x.Count == 2))
            {
                _info.Text += c.Name + "(" + c.Count + ")\n";
                Dupes.Add(c);
            }

        }

        public static void CheckForDupe(Card card)
        {
            if (HasReno == false) return;
            var dupeNames = Dupes.Select(x => x.Name).ToList();

            if (dupeNames.Contains(card.Name))
            {
                Logger.WriteLine("Getting Closer... " + card.Name + " removed.", "RenoTime");
                var cardToRemove = Dupes.FirstOrDefault(x => x.Name == card.Name);
                Dupes.Remove(cardToRemove);

                _info.Text = string.Empty;
                foreach (var c in Dupes)
                {
                    _info.Text += c.Name + "\n";
                }

                if(Dupes.Count == 0) //RENO TIME
                {
                    _info.Text = "Reno is hawt!";
                    _info.Fill = Brushes.LimeGreen;
                    
                    Logger.WriteLine("RENO TIME !!!!", "RenoTime");
                }
            }
        }

    }
}
