using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace GymLeaderTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        /// <summary>
        /// Gymline
        /// </summary>
        private readonly Button[] RegularGyms, AdvancedGyms;
        /// <summary>
        /// List of Challengers
        /// </summary>
        private readonly List<Challenger> Challengers = new List<Challenger>();
        /// <summary>
        /// Challenger that is being modified
        /// </summary>
        private Challenger SelectedChallenger = new Challenger();
        /// <summary>
        /// Timer to keep track of time
        /// </summary>
        private readonly System.Timers.Timer Timer = new System.Timers.Timer();
        /// <summary>
        /// SelectedIndex in <see cref="ChallengersListBox"/>
        /// </summary>
        private int SelectedIndex = 0;
        #endregion

        #region OnStartUp
        public MainWindow()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");

            RegularGyms = new Button[]
            {
                RegularGymOne, RegularGymTwo, RegularGymThree, RegularGymFour,
                RegularGymFive, RegularGymSix, RegularGymSeven, RegularGymEight
            };
            AdvancedGyms = new Button[]
            {
                AdvancedGymOne, AdvancedGymTwo, AdvancedGymThree, AdvancedGymFour,
                AdvancedGymFive, AdvancedGymSix, AdvancedGymSeven, AdvancedGymEight
            };

            SetUI(false);
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }
        /// <summary>
        /// Sets Gymlines & Minute inputs, adds another line to <see cref="ChallengersListBox"/> if needed, resets <see cref="ChallengerTextBox"/>
        /// </summary>
        /// <param name="Autocalled">Wether method was autocalled via <see cref="ChallengersListBox_SelectionChanged(object, SelectionChangedEventArgs)"/></param>
        private void SetUI(bool Autocalled)
        {
            SetRegularGyms("Electric", "Bug", "Fairy", "Steel", "Water", "Dark", "Flying", "Dragon");
            SetAdvancedGyms("Dark/Poison", "Fire/Dragon", "Electric/Ice", "Psychic/Dragon", "Water/Steel", "Electric/Flying", "Psychic/Fairy", "Steel/Fairy");
            SetMinuteListBoxItems();
            ChallengerTextBox.Text = "Challenger IGN";
            //If SetUI wasn't autocalled, add a new line
            if (!Autocalled) ChallengersListBox.Items.Add("");
            //else Selection_Changed() ran ArugmentOutOfRange exception, being unable to find a challenger in empty row
            else SelectedChallenger = new Challenger();
            ChallengersListBox.SelectedIndex = ChallengersListBox.Items.Count - 1;
        }
        /// <summary>
        /// Sets regular gymline
        /// </summary>
        /// <param name="GymLine">Gymline types</param>
        private void SetRegularGyms(params string[] GymLine) => SetGymLine(GymLine, RegularButton, RegularGyms);
        /// <summary>
        /// Sets advanced gymline
        /// </summary>
        /// <param name="GymLine">Gymline types</param>
        private void SetAdvancedGyms(params string[] GymLine) => SetGymLine(GymLine, AdvancedButton, AdvancedGyms);
        /// <summary>
        /// Handles <see cref="SetRegularGyms(string[])"/> & <see cref="SetAdvancedGyms(string[])"/>
        /// </summary>
        /// <param name="GymLine">Gymline to set</param>
        /// <param name="Buttons">Buttons to set</param>
        private void SetGymLine(string[] GymLine, Button GymType, Button[] Buttons)
        {
            GymType.IsEnabled = true;
            for (int x = 0; x < Buttons.Length; x++)
            {
                Buttons[x].Content = GymLine[x];
                Buttons[x].IsEnabled = false;
            }
        }
        /// <summary>
        /// Sets <see cref="MinuteListBox"/> items from 1 to 30, and default selects 30
        /// </summary>
        private void SetMinuteListBoxItems()
        {
            MinuteListBox.Items.Clear();
            for (int x = 30; x > 0; x--)
                MinuteListBox.Items.Add(x);
            MinuteListBox.SelectedItem = 30;
        }
        #endregion

        #region Auto Events
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //UI needs to be updated
                Dispatcher.Invoke(() =>
                {
                    //Re-add all items from this.Challengers to ChallengersListBox.Items
                    ChallengersListBox.Items.Clear();
                    for (int x = 0; x < Challengers.Count; x++)
                    {
                        ChallengersListBox.Items.Add(Challengers[x].GetToString());
                        //If dupelicate, remove dupelicate at this.Challengers & ChallengersListBox.Items
                        if (x >= 1 && ChallengersListBox.Items[x - 1].ToString() == ChallengersListBox.Items[x].ToString())
                        {
                            Challengers.RemoveAt(x);
                            ChallengersListBox.Items.RemoveAt(x);
                        }
                    }
                    //Add empty challenger
                    ChallengersListBox.Items.Add("");
                });
            }
            catch (System.Threading.Tasks.TaskCanceledException) { } //If user shutsdown program, this runs for some reason
            catch (ArgumentOutOfRangeException) { MessageBox.Show("ArgumentOutOfRange Exception occured in Timer_Elapsed"); }
        }
        private void ChallengersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Remember SelectedIndex, as SelectionChanged() runs every Timer_Elapsed() run
            if (ChallengersListBox.SelectedIndex == -1 || 
                ChallengersListBox.SelectedIndex == SelectedIndex)
            {
                ChallengersListBox.SelectedIndex = SelectedIndex;
                return;
            }

            //User selected a new index
            SelectedIndex = ChallengersListBox.SelectedIndex;
            //Try finding a challenger corresponding to user index, and update UI to challenger data
            try
            {
                SelectedChallenger = Challengers[SelectedIndex];
                ChallengerTextBox.Text = SelectedChallenger.Name;
                GymType_Clicked(new Button() { Content = SelectedChallenger.GymType }, null);
                Gym_Clicked(new List<Button>(RegularGyms)
                    .AddInto(AdvancedGyms)
                    .Find(btn => btn.Content.ToString() == SelectedChallenger.Gym), null);
            }
            //Couldn't find Challenger in this.Challengers - SelectedChallenger gets reset with UI
            catch (ArgumentOutOfRangeException) { SetUI(true); }
            catch (Exception ex) { throw ex; }
        }
        #endregion

        #region Gyms
        private void GymType_Clicked(object sender, RoutedEventArgs e)
        {
            //If sender is RegularButton
            if (RegularButton.Content.ToString().Contains((sender as Button).Content.ToString()))
            {
                //Disable RegularButton, enable AdvancedButton
                RegularButton.IsEnabled = false;
                AdvancedButton.IsEnabled = true;
            }
            //sender is AdvancedButton
            else
            {
                //Enable RegularButton, disable AdvancedButton
                RegularButton.IsEnabled = true;
                AdvancedButton.IsEnabled = false;
            }

            //If sender is RegularButton, enable all RegularGym buttons
            for (int x = 0; x < RegularGyms.Length; x++)
                RegularGyms[x].IsEnabled = (sender as Button).Content.ToString().Contains("Regular");
            //If sender is AdvancedButton, enable all AdvancedGym buttons
            for (int x = 0; x < AdvancedGyms.Length; x++)
                AdvancedGyms[x].IsEnabled = (sender as Button).Content.ToString().Contains("Advanced");
        }
        private void Gym_Clicked(object sender, RoutedEventArgs e)
        {
            Button caller = sender as Button;
            //If caller is RegularButton
            if (RegularGyms.Contains(caller.Content.ToString()))
            {
                //Enable all RegularButtons
                for (int x = 0; x < RegularGyms.Length; x++)
                    RegularGyms[x].IsEnabled = true;
                //Set GymType
                SelectedChallenger.GymType = "Regular";
            }
            else
            {
                //Enable all AdvancedButtons
                for (int x = 0; x < AdvancedGyms.Length; x++)
                    AdvancedGyms[x].IsEnabled = true;
                //Set GymType
                SelectedChallenger.GymType = "Advanced";
            }
            //Caller is a gym button, disable button so user can see which gym they selected
            caller.IsEnabled = false;
            //Set Gym
            SelectedChallenger.Gym = caller.Content.ToString();
        }
        #endregion

        #region Start/Remove
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //Any of these return true, user didn't provide enough input
            if (SelectedChallenger.Gym is null ||
                ChallengerTextBox.Text == "Challenger IGN" ||
                ChallengerTextBox.Text == string.Empty)
            {
                MessageBox.Show("Not all requirements were met");
                return;
            }

            //Set SelectedChallenger
            SelectedChallenger.Name = ChallengerTextBox.Text;
            SelectedChallenger.EndTime = DateTime.Now.AddMinutes(int.Parse(MinuteListBox.Items[MinuteListBox.SelectedIndex].ToString()));

            //If start was used to update a previous Challenger, replace previous value
            try { ChallengersListBox.Items[ChallengersListBox.Items.Count - 1] = SelectedChallenger.ToString(); }
            //Challenger is a new challenger
            catch (ArgumentOutOfRangeException)
            {
                //If challenger is not a duplicate, add challenger to ListBox
                if (!ChallengersListBox.Items.Contains(SelectedChallenger.GetToString()))
                    ChallengersListBox.Items.Add(SelectedChallenger.ToString());
            }
            finally
            {
                //If challenger is not a duplicate, add chalenger to this.Challengers
                if (!Challengers.Contains(SelectedChallenger))
                    Challengers.Add(SelectedChallenger);
            }
            //Reset SelectedChallenger
            SelectedChallenger = new Challenger();
            //Reset UI
            SetUI(false);
        }
        private void RemoveChallengerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Remove challenger using SelectedIndex
                Challengers.RemoveAt(SelectedIndex);
                ChallengersListBox.Items.RemoveAt(SelectedIndex);
                //Reset UI
                SetUI(false);
            }
            //No Challenger selected - unable to remove
            catch (ArgumentOutOfRangeException) { MessageBox.Show("There is nothing to remove!"); }
        }
        #endregion
    }
}