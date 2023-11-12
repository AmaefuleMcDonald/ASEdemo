using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using static MyWork.Form1;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing.Text;
using System.CodeDom;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices;


namespace MyWork
{
   /// <summary>
   /// Represents the main form of the application for drawing shapes
   /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Confirm if the circle has been drawn
        /// </summary>
        private bool circleDrawn = false;
        /// <summary>
        /// Reoresents the diameter of the drawn circle
        /// </summary>       
        private int circleDiameter = 0; 
        /// <summary>
        /// Stores the content of the last displayed messagebox
        /// </summary>
        private string lastMessageBoxContent;
        /// <summary>
        /// The action which happens when button is clicked
        /// </summary>
        public Action buttonClickAction;
        /// <summary>
        /// Event handler for the paint event of the picture box
        /// </summary>
        private PaintEventHandler pictureBox1_Paint;
        /// <summary>
        /// Handles syntax checking for commands
        /// </summary>
        private SyntaxChecker syntaxChecker; 
        /// <summary>
        /// Represents the menu strip for the form
        /// </summary>
        private MenuStrip menuStrip1;
        /// <summary>
        /// Represents the pen used for drawing shapes
        /// </summary>
        private Pen pen; //declare the pen
        /// <summary>
        /// Represents a black pen used for drawing
        /// </summary>
        private Pen blackPen; 
        /// <summary>
        /// Represents bitmap used to store the drawing
        /// </summary>
        private Bitmap drawingArea; 
        /// <summary>
        /// Represents the position of the pen
        /// </summary>
        private Point penPosition;
        /// <summary>
        /// Represents the ToolStripMenuItem for loading drawings
        /// </summary>
        private ToolStripMenuItem loadToolStripMenuItem;
        /// <summary>
        /// Represents the ToolStripMenuItem for saving drawings
        /// </summary>
        private ToolStripMenuItem saveToolStripMenuItem;
        /// <summary>
        /// Indicates wether shapes should be filled when drawn
        /// </summary>
        private bool fillShapes;

        /// <summary>
        /// Loads drawing related settings from file
        /// </summary>
        /// <param name="fileName">The name of the file to load settings from</param>
        /// <remarks>
        /// penPosition: sets the penposition (x, y)
        /// penColor: sets the pen color
        /// fillShapes: sets wether shapes should be filled
        /// </remarks>    
        private void LoadFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(' ');
                        if (parts[0] == "penPosition" && parts.Length == 3 &&
                            int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
                        {
                            penPosition = new Point(x, y);
                        }
                        else if (parts[0] == "penColor" && parts.Length == 2)
                        {
                            pen.Color = Color.FromName(parts[1]);
                        }
                        else if (parts[0] == "FillShapes" && parts.Length == 2 && bool.TryParse(parts[1], out bool fill))
                        {
                            fillShapes = fill;
                        }
                    }
                }

            }


        }
        /// <summary>
        /// saves drawing related settings to file
        /// </summary>
        /// <param name="fileName">The name of the file to save settings to</param>
        private void SaveToFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                //save current state of program
                sw.WriteLine($"PenPosition {penPosition.X} {penPosition.Y}");
                sw.WriteLine($"PenColor {pen.Color.Name}");
                sw.WriteLine($"FillShapes {fillShapes}");
            }
        }

        /// <summary>
        /// Handles click for load, which allows the loading of drawing settings from a file
        /// </summary>
        /// <param name="sender">The object that triggered the event</param>
        /// <param name="e">an EventArgs object which contains event data</param>
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //create and configure an open file dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            //performs check to confirm if a user selected a file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //load drawing settings from the file selected
                LoadFromFile(openFileDialog.FileName);
                // Refresh the PictureBox to reflect the loaded settings
                pictureBox1.Refresh();
            }
        }
        /// <summary>
        /// Handles click for save, which allows the saving of drawing settings to a file
        /// </summary>
        /// <param name="sender">The object that triggered the event</param>
        /// <param name="e">an EventArgs object which contains event data</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveToFile(saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Executes a drawing command based on the string command typed in
        /// </summary>
        /// <param name="command">The input command to be executed</param>
        /// <remarks>
        /// The method parses the input command, checks its syntax, and executes the corresponding drawing operation.
        /// Supported commands include:
        ///  "moveTo x y": Moves the pen to the specified coordinates (x, y).
        ///  "drawTo x y": Draws a line from the current pen position to the specified coordinates (x, y).
        ///  "clear": Clears the graphics area.
        ///  "reset": Resets the pen position.
        ///  "Circle radius": Draws a circle with the specified radius.
        ///  "Rectangle width height": Draws a rectangle with the specified width and height.
        ///  "Triangle side1 side2": Draws a triangle with the specified side lengths.
        ///  "blue": Sets the pen color to blue.
        ///  "green": Sets the pen color to green.
        ///  "red": Sets the pen color to red.
        /// </remarks>
        public void ExecuteCommand(string command)
            {
            try
            {
                syntaxChecker.CheckSyntax(command); //check the syntax of the command
                string[] input = command.Split(' ');
                buttonClickAction?.Invoke();
                // If the command is executed successfully, reset the error message
                ShowMessageBox(""); // Empty string or null to reset the error message

                switch (input[0])
                {
                    case "moveTo":
                        if (input[0].StartsWith("moveTo") && input.Length == 3 && int.TryParse(input[1], out int x) && int.TryParse(input[2], out int y))
                        {
                            penPosition = new Point(x, y);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid syntax for moveTo command.");
                        }
                        break;
                    //default:
                        //throw new ArgumentException("Invalid syntax for moveTo command.");                
                 case "drawTo":
                    if (input.Length == 3 && int.TryParse(input[1], out int x2) && int.TryParse(input[2], out int y2))
                    {
                        DrawToPosition(x2, y2);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid syntax for drawTo command.");
                    }
                    break;

                    case "clear":                     
                            ClearGraphicsArea();
                        break;
                    case "reset":                       
                            ResetPenPosition();
                        break;

                    case "Circle":
                        if (input[0] == "Circle" && input.Length == 2 && int.TryParse(input[1], out int radius))
                        {
                            DrawCircle(radius);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid syntax for Circle command.");
                        }
                        break;

                    case "Rectangle":
                        if (input[0] == "Rectangle" && input.Length == 3 && int.TryParse(input[1], out int width) && int.TryParse(input[2], out int height))
                        {
                            DrawRectangle(width, height);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid syntax for Rectangleo command.");
                        }
                        break;

                    case "Triangle":
                        if (input[0] == "Triangle" && input.Length == 3 && int.TryParse(input[1], out int side1) && int.TryParse(input[2], out int side2))
                        {
                            DrawTriangle(side1, side2);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid syntax for drawTo command.");
                        }
                        break;

                    case "blue":
                        SetPenColor(Color.Blue); 
                        break;

                    case "green":
                        SetPenColor(Color.Green); 
                        break;

                    case "red":
                        SetPenColor(Color.Red);
                        break;
                }
                
            }

            catch (ArgumentException ex)
            {
                // If there's an exception, set the error message
                ShowMessageBox(ex.Message);             
            }
            }
        /// <summary>
        /// Handles the keypress event for textBox1, focusing on the enter key
        /// </summary>
        /// <param name="sender">The object that triggered the event</param>
        /// <param name="e">A KeyPressEventArgs object containing event data.</param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string command = textBox1.Text;
                ExecuteCommand(command);
                button1.PerformClick();
                e.Handled = true;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        ///  <remarks>
        /// This constructor sets up the initial configuration for the Form1 instance, including initializing components, attaching event handlers,
        ///  setting up the drawing area, configuring the menu strip, and initializing the syntax checker.
        /// </remarks>
        public Form1()
        {
            // initialise component using the designer-generated code
            InitializeComponent();
            //Attach the Form1_Load eventhandler
            this.Load += Form1_Load;
            //Create a black pen for drawing
            blackPen = new Pen(Color.Black);
            // Attach the PictureBox1_Paint event handler to the Paint event of the PictureBox
            pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox1_Paint);
            // Attach the button1_Click event handler to the Click event of button1 
            button1.Click += new System.EventHandler(this.button1_Click);
            //the initial position of the pen
            penPosition = new Point(10, 10);
            // the initial color of the pen
            pen = blackPen;
            // attach the textbox1 event handler to the keyPress event to textBox1 
            textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);
            // Initialise toomStripMenuItems for "Load" and "Save"
            loadToolStripMenuItem = new ToolStripMenuItem("Load");
            saveToolStripMenuItem = new ToolStripMenuItem("Save");
            // Attach the loadToolStripMenuItem_Click event handler to the click event of loadToolStripMenuItem
            loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItem_Click);
            // Attach the saveToolStripMenuItem_Click event handler to the click event of saveToolStripMenuItem
            saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            //create and configure the menuStrip
            menuStrip1 = new MenuStrip();
            this.MainMenuStrip = menuStrip1;
            this.Controls.Add(menuStrip1);
            //Add toolStripMenu Items to the menuStrip
            menuStrip1.Items.Add(loadToolStripMenuItem);
            menuStrip1.Items.Add(saveToolStripMenuItem);
            //Add the pictureBox to the Form1 controls
            this.Controls.Add(this.pictureBox1);
            //Initialize the drawing area as a Bitmap with the size of the PictureBox
            drawingArea = new Bitmap(pictureBox1.Width, pictureBox1.Height);
           // creates a Graphics object for drawing on pictueBox
            graphics = pictureBox1.CreateGraphics();
            //Initialize the syntax checker
            syntaxChecker = new SyntaxChecker();
            // Attach the Syntax_Click event handler to the click event of the Syntax button
            Syntax.Click += new System.EventHandler(this.Syntax_Click);
            //Attach thebutton1_Click event handler to the click event of the Run button
            button1.Click += new System.EventHandler(this.button1_Click);
        }
        /// <summary>
        /// Handles the click event of the syntax button, checking the syntax command in textBox1
        /// </summary>
        /// <param name="sender">The object that triggered the event</param>
        /// <param name="e">an EventArgs object containing the event data</param>
        private void Syntax_Click(object sender, EventArgs e)
        {
            try
            {
                string command = textBox1.Text;
                syntaxChecker.CheckSyntax(command);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       /// <summary>
       ///Handles the valid commands in the textBox.
       /// </summary>
        public class SyntaxChecker
        {
            private HashSet<string> validCommands; //list of valid commands
            public SyntaxChecker()
            {
                validCommands = new HashSet<string>
            {
                "moveTo",
                "drawTo",
                "Rectangle",
                "Circle",
                "clear",
                "Triangle",
                "green",
                "reset",
                "red",
                "blue",
                "fill"
            };
            }
            /// <summary>
            /// Handles the syntax of the provided drawing command
            /// </summary>
            /// <param name="command">The drawing command to be validated</param>
            /// <exception cref="ArgumentException">The error message to be thrown when an invalid command is entered</exception>
             public void CheckSyntax(string command)
             {
                 string[] input = command.Split(' ');
                 if (input.Length == 0)
                 {
                     throw new ArgumentException("Empty command. Please enter a valid command.");
                 }
                 if (!validCommands.Contains(input[0]))
                 {
                     throw new ArgumentException("Invalid command. Please enter a valid command.");
                 }
                 switch (input[0])
                 {
                     case "drawTo":
                         if (input.Length != 3 || !int.TryParse(input[1], out _) || !int.TryParse(input[2], out _))
                         {
                             throw new ArgumentException("Invalid syntax for drawTo command.");
                         }
                         break;

                     case "moveTo":
                        if (input.Length != 3 || !int.TryParse(input[1], out _) || !int.TryParse(input[2], out _))
                         {
                             throw new ArgumentException("Invalid syntax for moveTo command.");
                         }
                         break;
                 }
             }
            
        }

        /// <summary>
        /// Handles the Paint event for the PictureBox, drawing a small square at the top-left corner based on the current pen position.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">A PaintEventArgs object containing event data.</param>
        /// <remarks>
        /// This method uses the Graphics object from the PaintEventArgs to draw a small black square at the current pen position (penPosition).
        /// The square has a size of 5x5 pixels.
        /// </remarks>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">A PaintEventArgs object containing event data.</param>
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //A small square at the top left corder of the picturebox
            e.Graphics.FillRectangle(Brushes.Black, penPosition.X, penPosition.Y, 5, 5);
        }
        /// <summary>
        /// The Graphics object used for drawing on the PictureBox.
        /// </summary>
        private Graphics graphics;
        /// <summary>
        /// indicates if the error message has been thrown
        /// </summary>
        private bool errorDisplayed = false;
        /// <summary>
        /// Event handler for the Form1_Load event.
        /// </summary>
        private EventHandler Form1_Load;

        /// <summary>
        /// Handles the Click event for the button1 control.
        /// Calls the <see cref="DoSave"/> method to process and save the drawing command.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">An EventArgs object containing event data.</param>
        private void button1_Click(object sender, EventArgs e) 
        {
            //call the DoSave method to process and save the drawing command
            DoSave();
        }
        /// <summary>
        /// Processes and saves the drawing command entered in the textBox1 control.
        /// </summary>
        /// <remarks>
        /// This method extracts the drawing command from textBox1, checks its syntax using <see cref="SyntaxChecker.CheckSyntax"/>,
        /// and executes the corresponding drawing operation. Supported commands include "Circle," "Rectangle," "Triangle," "clear," "reset,"
        /// "moveTo," "drawTo," "red," "green," "blue," and "fill."
        /// If an ArgumentException is caught during processing, it is handled by the <see cref="HandleArgumentException"/> method.
        /// </remarks>
        public void DoSave()
        { 
            
   
            {
                try
                {
                    //extract the command from textBox1
                    string command = textBox1.Text;
                    //check the syntax of the command
                    syntaxChecker.CheckSyntax(command);
                    //initialize the drawing area if it is null
                    if (pictureBox1.Image == null)
                    {
                        pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    }

                    graphics = Graphics.FromImage(pictureBox1.Image);

                    string[] input = textBox1.Text.Split(' ');
                    if (input[0] == "Circle" && input.Length == 2 && int.TryParse(input[1], out int radius))
                    {
                        DrawCircle(radius);
                    }
                    else if (input[0] == "Rectangle" && input.Length == 3 && int.TryParse(input[1], out int width) && int.TryParse(input[2], out int height))
                    {
                        DrawRectangle(width, height);
                    }

                    else if (input[0] == "Triangle" && input.Length == 3 && int.TryParse(input[1], out int side1) && int.TryParse(input[2], out int side2))
                    {
                        DrawTriangle(side1, side2);
                    }
                    else if (input[0] == "clear")
                    {
                        ClearGraphicsArea();
                    }
                    else if (input[0] == "reset")
                    {
                        ResetPenPosition();
                    }
                    else if (input[0].StartsWith("moveTo") && input.Length == 3 && int.TryParse(input[1], out int x) && int.TryParse(input[2], out int y))
                    {
                        MoveToPosition(x, y);
                    }
                    else if (input[0].StartsWith("drawTo") && input.Length == 3 && int.TryParse(input[1], out int x2) && int.TryParse(input[2], out int y2))
                    {
                        DrawToPosition(x2, y2);
                    }
                    else if (input[0] == "red")
                    {
                        SetPenColor(Color.Red);
                    }
                    else if (input[0] == "green")
                    {
                        SetPenColor(Color.Green);
                    }
                    else if (input[0] == "blue")
                    {
                        SetPenColor(Color.Blue);
                    }
                    else if (input[0] == "fill" && input.Length == 2)
                    {
                        SetFillShapes(input[1]);
                    }
                    pictureBox1.Refresh();
                    errorDisplayed = false;
                }

                catch (ArgumentException ex)
                {
                    
                    HandleArgumentException(ex);

                }

                
            }
        }

        /// <summary>
        /// Draws a circle
        /// </summary>
        /// <param name="radius">the radius of the circle to be drawn</param>
        public void DrawCircle(int radius)
        {
            try
            {
                if (fillShapes)
                {
                    graphics.FillEllipse(pen.Brush, 10, 10, radius * 2, radius * 2);
                }
                else
                {
                    graphics.DrawEllipse(pen, 10, 10, radius * 2, radius * 2);
                }
                circleDrawn = true;
                circleDiameter = radius * 2;
                MessageBox.Show("Circle drawn successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           
        }
        /// <summary>
        /// checks if circle is drawn
        /// </summary>
        /// <returns> <c>true</c> if a circle has been drawn; otherwise, <c>false</c>.</returns>
        public bool IsCircleDrawn()
        {

            return circleDrawn;
        }
        /// <summary>
        /// Gets the diameter of the circle drawn 
        /// </summary>
        /// <returns> <c>true</c> if diameter is valid; otherwise, <c>false</c>.</returns>
        public int GetCircleDiameter()
        {
            return circleDiameter;
        }
        /// <summary>
        /// Draws a rectangle
        /// </summary>
        /// <param name="width">width of rectangle drawn</param>
        /// <param name="height">height of rectangle drawn</param>
        private void DrawRectangle(int width, int height)
        {
                        if (fillShapes)
                        {
                            graphics.FillRectangle(pen.Brush, 10, 10, width, height);

                        }
                        else
                        {
                            graphics.DrawRectangle(pen, 10, 10, width, height);
                        }
                    }
        /// <summary>
        /// Draws a Triangle
        /// </summary>
        /// <param name="side1">side 1 of the triangle</param>
        /// <param name="side2">side 2 of the triangle</param>
        private void DrawTriangle(int side1, int side2)
        {
                        if (fillShapes)
                        {
                            Point point1 = new Point(10, 10 + side2);
                            Point point2 = new Point(10 + side1, 10 + side2);
                            Point point3 = new Point(10 + side1 / 2, 10);

                            Point[] trianglePoints = { point1, point2, point3 };

                            graphics.FillPolygon(pen.Brush, trianglePoints);
                        }
                        else
                        {
                            Point point1 = new Point(10, 10 + side2);
                            Point point2 = new Point(10 + side1, 10 + side2);
                            Point point3 = new Point(10 + side1 / 2, 10);

                            Point[] trianglePoints = { point1, point2, point3 };

                            graphics.DrawPolygon(pen, trianglePoints);
                        }
                    }
        /// <summary>
        /// clears the graphic area
        /// </summary>
        private void ClearGraphicsArea()
        {
                        //Clear the graphics area
                        graphics.Clear(Color.Transparent);
                    }
        /// <summary>
        /// Resets the initial penposition 
        /// </summary>
        private void ResetPenPosition()
        {
                        penPosition = new Point(10, 10);
                    }
        /// <summary>
        /// Move pen position
        /// </summary>
        /// <param name="x">position x</param>
        /// <param name="y">position y</param>
        private void MoveToPosition(int x, int y)
        {
                        penPosition = new Point(x, y);
                    }
        /// <summary>
        /// drawTo penPosition
        /// </summary>
        /// <param name="x2">position x2</param>
        /// <param name="y2">position y2</param>
        private void DrawToPosition(int x2, int y2)
        {
                        Point endPoint = new Point(x2, y2);
                        graphics.DrawLine(pen, penPosition, endPoint);
                        penPosition = endPoint; // update the pen position
                    }
        /// <summary>
        /// sets pen color
        /// </summary>
        /// <param name="color">name of color the pen will be set to</param>
        private void SetPenColor(Color color)
        {
            pen.Color = color;
        }
        /// <summary>
        /// set fill shapes to on or off
        /// </summary>
        /// <param name="input">input to set fill on or off</param>
      private void SetFillShapes(string input)
        {
                        if (input == "on")
                        {
                            fillShapes = true;
                        }
                        else if (input == "off")
                        {
                            fillShapes = false;
                        }
                    }
        /// <summary>
        /// Handles an ArgumentException by displaying an error message in a MessageBox if it hasn't been displayed before.
        /// </summary>
        /// <param name="ex">The ArgumentException to handle.</param>
        private void HandleArgumentException (ArgumentException ex)
       {
         if (!errorDisplayed)
         {
         MessageBox.Show(ex.Message, "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         errorDisplayed = true;
        }
    }
        /// <summary>
        /// Handles the TextChanged event for the richTextBox1 control.
        /// </summary>
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Gets the content of the last displayed messageBox
        /// </summary>
        /// <returns>A string representing the content of the last displayed MessageBox.</returns>
        public string GetLastMessageBoxContent()
        {
            return lastMessageBoxContent;
        }
        /// <summary>
        /// Shows the message box
        /// </summary>
        /// <param name="message">Simulate showing a message</param>
        public void ShowMessageBox(string message)
        {
            // Simulate showing a message box and store the content
            lastMessageBoxContent = message;
        }
        /// <summary>
        /// Gets the current penPosition on the drawing surface
        /// </summary>
        /// <returns>the point representing the current position</returns>
        public Point GetPenPosition()
        {
            return penPosition;
        }
        /// <summary>
        /// gets the current penColor on the drawing surface
        /// </summary>
        /// <returns>the color of the pen</returns>
        public Color GetPenColor()
        {
            return pen.Color;
        }

        /// <summary>
        /// Retrieves the current fill shapes status.
        /// </summary>
        /// <returns>
        /// <c>true</c> if fill shapes are enabled; otherwise, <c>false</c>.
        public bool GetFillShapes()
        {
            return fillShapes;
        }
        /// <summary>
        /// Handles the Click event for the Syntax control.
        /// </summary>
        private void Syntax_Click_1(object sender, EventArgs e)
        {

        }
    }
}


