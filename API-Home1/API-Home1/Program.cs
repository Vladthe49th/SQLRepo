using System;
using System.Runtime.InteropServices;



// Main task
class Program
{
    // Messagebox import
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    const uint MB_YESNO = 0x00000004;
    const uint MB_YESNOCANCEL = 0x00000003;
    const uint MB_ICONQUESTION = 0x00000020;
    const uint MB_ICONINFORMATION = 0x00000040;

    const int IDYES = 6;
    const int IDNO = 7;
    const int IDCANCEL = 2;

    static void Main()
    {
        do
        {
            PlayGame();
        } while (WantsToPlayAgain());
    }

    static void PlayGame()
    {
        int min = 0, max = 100;
        int guess;
        int response;

        MessageBox(IntPtr.Zero, "Make up a bumber from 0 to 100, then press OK", "Guess the num", MB_ICONINFORMATION);

        while (true)
        {
            guess = (min + max) / 2;
            response = MessageBox(IntPtr.Zero, $"Your number is: {guess}?\n\nYes - guessed\nNo - I`ll give a clue", "Attempt", MB_YESNO | MB_ICONQUESTION);

            if (response == IDYES)
            {
                MessageBox(IntPtr.Zero, "Yay! I, the computer, guessed your number.", "Victory", MB_ICONINFORMATION);
                break;
            }
            else
            {
                // Is the num bigger?
                response = MessageBox(IntPtr.Zero, "Is your num bigger?", "Clue", MB_YESNO | MB_ICONQUESTION);

                if (response == IDYES)
                {
                    min = guess + 1;
                }
                else
                {
                    max = guess - 1;
                }

                if (min > max)
                {
                    MessageBox(IntPtr.Zero, "I think there`s a mess in your head, let`s try again", "Error", MB_ICONINFORMATION);
                    break;
                }
            }
        }
    }

    static bool WantsToPlayAgain()
    {
        int response = MessageBox(IntPtr.Zero, "Wanna play again?", "Repeat", MB_YESNO | MB_ICONQUESTION);
        return response == IDYES;
    }


}



