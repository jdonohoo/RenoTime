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
using RenoTime.Controls;


namespace RenoTime
{
    public class RenoCode
    {
        
        private static List<Card> Dupes { get; set; }
        private static Entity Player { get { return Entities == null ? null : Entities.First(x => x.IsPlayer); } }
        private static RenoUI RenoPanel { get; set; }
        public static bool HasReno { get { return DeckList.Instance.ActiveDeck.Cards.FirstOrDefault(x => x.Name == "Reno Jackson") == null ? false : true; } }
        
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

            // Register methods to be called when GameEvents occur
            GameEvents.OnGameStart.Add(NewGame);
            Hearthstone_Deck_Tracker.API.DeckManagerEvents.OnDeckSelected.Add(DeckSwitched);
            GameEvents.OnGameEnd.Add(GameOver);
            GameEvents.OnInMenu.Add(GameOver);
            GameEvents.OnPlayerDraw.Add(CardDrew);
            GameEvents.OnTurnStart.Add(TurnStart);


            InitUI();
            
        }


        private static void InitUI()
        {

            RenoPanel = new RenoUI();
            RenoPanel.RenoText.Text = "";
            RenoPanel.RenoText.FontSize = 18;
            RenoPanel.RenoText.Fill = Brushes.Yellow;


            // Get the HDT Overlay canvas object
            var canvas = Hearthstone_Deck_Tracker.API.Core.OverlayCanvas;
            
            // Get canvas centre
            var fromTop = canvas.Height / 2;
            var fromLeft = canvas.Width / 2;
            
            // Give the text block its position within the canvas, roughly in the center
            Canvas.SetTop(RenoPanel, fromTop);
            Canvas.SetLeft(RenoPanel, fromLeft);
            
                        
            // Add the text block and image to the canvas
            canvas.Children.Add(RenoPanel);
        }



        public static void NewGame()
        {
            Logger.WriteLine("NewGame();", "RenoTime");

            
            if(HasReno)
            {
                Dupes = new List<Card>();
                RenoPanel.Visibility = Visibility.Visible;
                RenoPanel.RenoText.Fill = Brushes.Yellow;
                LoadDupes();
                CheckDupes();
                
            }
            else
            {
                RenoPanel.Visibility = Visibility.Hidden;
            }           


        }

        public static void DeckSwitched(Deck deck)
        {
            NewGame();
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
            RenoPanel.Visibility = Visibility.Hidden;
        }

        public static void LoadDupes()
        {
            if (HasReno == false) return;
            Logger.WriteLine("LoadDupes();", "RenoTime");

            Dupes.Clear();
            var deck = DeckList.Instance.ActiveDeck;

            RenoPanel.RenoText.Text = string.Empty;

            foreach (var c in deck.Cards.Where(x=>x.Count == 2))
            {
                RenoPanel.RenoText.Text += c.Name + "\n";
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

                RenoPanel.RenoText.Text = string.Empty;
                foreach (var c in Dupes)
                {
                    RenoPanel.RenoText.Text += c.Name + "\n";
                }

                if(Dupes.Count == 0) //RENO TIME
                {
                    RenoPanel.RenoText.Text = "Reno is hawt!";
                    RenoPanel.RenoText.Fill = Brushes.LimeGreen;
                    
                    Logger.WriteLine("RENO TIME !!!!", "RenoTime");
                }
            }
        }

    }
}
