/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace SnakeGame
{
    public partial class Konami
    {
        private readonly List<Key> konamiCode = new List<Key>
        {
            Key.Up, Key.Up,
            Key.Down, Key.Down,
            Key.Left, Key.Right,
            Key.Left, Key.Right,
            Key.B, Key.A
        };

        private readonly List<Key> userInput = new List<Key>();

       

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Handle KeyDown event
            userInput.Add(e.Key);

            // Check if the entered keys match the Konami Code
            if (CheckKonamiCode())
            {
                MessageBox.Show("Konami Code activated!");
                // Add your custom action here

                private async Task GameLoop()
                {
                    while (!gameState.GameOver)
                    {
                        await Task.Delay(25);
                        gameState.Move();
                        Draw();
                    }
                }
                // Reset for the next input
                userInput.Clear();
            }
        }

        private bool CheckKonamiCode()
        {
            // Check if the entered keys match the Konami Code
            if (userInput.Count > konamiCode.Count)
            {
                userInput.Clear(); // Reset if the entered keys are too many
            }

            for (int i = 0; i < userInput.Count; i++)
            {
                if (userInput[i] != konamiCode[i])
                {
                    userInput.Clear(); // Reset if there is a mismatch
                    return false;
                }
            }

            // Check if the entire Konami Code has been entered
            return userInput.Count == konamiCode.Count;
        }
    }
}
*/
