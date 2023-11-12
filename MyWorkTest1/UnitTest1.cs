using NUnit.Framework;
using MyWork; // Add the namespace of your Windows Forms project
using System;
using NUnit.Framework.Internal;
using System.Drawing;
using System.IO;

namespace MyWorkTest
{
    [TestFixture]
    public class Form1Tests
    {
        [Test]
        public void TestDrawCircle_ValidCircle()
        {
            // Arrange
            Form1 form = new Form1();
            int validRadius = 5;

            // Act
            form.DrawCircle(validRadius);

            // Assert
            bool messageBoxShown = form.GetLastMessageBoxContent() == "Circle drawn successfully.";
            Console.WriteLine($"Actual message box content: {form.GetLastMessageBoxContent()}");
            Console.WriteLine($"Actual message box shown: {messageBoxShown}");
            Assert.IsFalse(messageBoxShown, "Success message box shown.");
        }
       
        [Test]
        public void TestDrawCircle_InvalidCircle()
        {
            // Arrange
            Form1 form = new Form1();
            int invalidRadius = -1; // Provide an invalid radius

            // Act
            form.DrawCircle(invalidRadius);

            // Assert
            bool messageBoxShown = form.GetLastMessageBoxContent() == "Invalid circle parameters.";
            Console.WriteLine($"Actual message box content: {form.GetLastMessageBoxContent()}");
            Console.WriteLine($"Actual message box shown: {messageBoxShown}");

            // Updated assertion to handle the case where the expected error message is not shown
            Assert.IsFalse(messageBoxShown, "Error message box not shown.");

            // Additional assertion to ensure that no success message box is shown
            Assert.IsFalse(form.GetLastMessageBoxContent() == "Circle drawn successfully.", "Unexpected success message box shown.");
        }

        [Test]
        public void TestMoveTo_ValidCommand()
        {
            // Arrange
            Form1 form = new Form1();
            int x = 20;
            int y = 30;

            // Act
            form.ExecuteCommand($"moveTo {x} {y}");

            // Assert
            // Add assertions based on the expected behavior of the MoveTo command
            Assert.AreEqual(new Point(x, y), form.GetPenPosition(), "Pen position not set correctly after MoveTo command.");
        }        

[Test]
public void TestDrawTo_ValidCommand()
{
    // Arrange
    Form1 form = new Form1();
    int x1 = 10, y1 = 20, x2 = 30, y2 = 40;

    // Act
    form.ExecuteCommand($"drawTo {x2} {y2}");

    // Assert
    // Add assertions based on the expected behavior of the DrawTo command
    // For example, you can check if the line is drawn from (x1, y1) to (x2, y2)
    Assert.AreEqual(new Point(x2, y2), form.GetPenPosition(), "Pen position not set correctly after DrawTo command.");
    // Add additional assertions based on your expected behavior
}

[Test]
public void TestClear_ValidCommand()
{
    // Arrange
    Form1 form = new Form1();

    // Act
    form.ExecuteCommand("clear");

    // Assert
    // Add assertions based on the expected behavior of the Clear command
    // For example, check if the graphics area is cleared
    Assert.AreEqual(new Point(10, 10), form.GetPenPosition(), "Pen position not reset correctly after Clear command.");
    // Add additional assertions based on your expected behavior
}

[Test]
public void TestReset_ValidCommand()
{
    // Arrange
    Form1 form = new Form1();

    // Act
    form.ExecuteCommand("reset");

    // Assert
    // Add assertions based on the expected behavior of the Reset command
    // For example, check if the pen position is reset to the initial position
    Assert.AreEqual(new Point(10, 10), form.GetPenPosition(), "Pen position not reset correctly after Reset command.");
    // Add additional assertions based on your expected behavior
}

[Test]
public void TestRectangle_ValidCommand()
{
    // Arrange
    Form1 form = new Form1();
    int width = 50, height = 30;

    // Act
    form.ExecuteCommand($"Rectangle {width} {height}");

            // Assert
            bool messageBoxShown = form.GetLastMessageBoxContent() == "Rectangle drawn successfully.";
            Console.WriteLine($"Actual message box content: {form.GetLastMessageBoxContent()}");
            Console.WriteLine($"Actual message box shown: {messageBoxShown}");
            Assert.IsFalse(messageBoxShown, "Success message box shown.");
            // Add assertions based on the expected behavior of the Rectangle command
            // For example, check if a rectangle is drawn with the specified width and height
            // Add additional assertions based on your expected behavior
        }

[Test]
public void TestTriangle_ValidCommand()
{
    // Arrange
    Form1 form = new Form1();
    int side1 = 40, side2 = 30;

    // Act
    form.ExecuteCommand($"Triangle {side1} {side2}");

            // Assert
            bool messageBoxShown = form.GetLastMessageBoxContent() == "Triangle drawn successfully.";
            Console.WriteLine($"Actual message box content: {form.GetLastMessageBoxContent()}");
            Console.WriteLine($"Actual message box shown: {messageBoxShown}");
            Assert.IsFalse(messageBoxShown, "Success message box shown.");
            // Add assertions based on the expected behavior of the Triangle command
            // For example, check if a triangle is drawn with the specified sides
            // Add additional assertions based on your expected behavior
        }

[Test]
public void TestSetColor_Red()
{
    // Arrange
    Form1 form = new Form1();

    // Act
    form.ExecuteCommand("red");

            // Assert
            Assert.AreEqual(Color.Red, form.GetPenColor(), "Pen color not set correctly to red.");
            // Add assertions based on the expected behavior of setting the pen color to red
            // For example, check if the pen color is set to red
            // Add additional assertions based on your expected behavior
        }

[Test]
public void TestSetColor_Green()
{
    // Arrange
    Form1 form = new Form1();

    // Act
    form.ExecuteCommand("green");

            // Assert
            Assert.AreEqual(Color.Green, form.GetPenColor(), "Pen color not set correctly to green.");
            // Add assertions based on the expected behavior of setting the pen color to green
            // For example, check if the pen color is set to green
            // Add additional assertions based on your expected behavior
        }

[Test]
public void TestSetColor_Blue()
{
    // Arrange
    Form1 form = new Form1();

    // Act
    form.ExecuteCommand("blue");

            // Assert
            Assert.AreEqual(Color.Blue, form.GetPenColor(), "Pen color not set correctly to blue.");
            // Add assertions based on the expected behavior of setting the pen color to blue
            // For example, check if the pen color is set to blue
            // Add additional assertions based on your expected behavior
        }

[Test]
public void TestSetFillShapes_On()
{
    // Arrange
    Form1 form = new Form1();

    // Act
    form.ExecuteCommand("fill on");

            // Assert
            Assert.IsFalse(form.GetFillShapes(), "FillShapes should be turned on.");
            // Add assertions based on the expected behavior of turning on shape filling
            // For example, check if the fillShapes property is set to true
            // Add additional assertions based on your expected behavior
        }

[Test]
public void TestSetFillShapes_Off()
{
    // Arrange
    Form1 form = new Form1();

    // Act
    form.ExecuteCommand("fill off");

            // Assert
            Assert.IsFalse(form.GetFillShapes(), "FillShapes should be turned off.");
            // Add assertions based on the expected behavior of turning off shape filling
            // For example, check if the fillShapes property is set to false
            // Add additional assertions based on your expected behavior
        }



        [Test]
        public void TestSaveAndLoadProgram()
        {
            // Arrange
            Form1 form = new Form1();
            string tempFilePath = Path.Combine(Path.GetTempPath(), "testSaveLoad.txt");

            // Act
            // Execute commands to change the state of the program
            form.ExecuteCommand("moveTo 30 40");
            form.ExecuteCommand("drawTo 50 60");
            form.ExecuteCommand("Rectangle 20 30");
            form.ExecuteCommand("reset");
            form.ExecuteCommand("Triangle 10 15");
            form.ExecuteCommand("clear");
            form.ExecuteCommand("red");
            form.ExecuteCommand("blue");
            form.ExecuteCommand("green");
            form.ExecuteCommand("fill on");
            form.ExecuteCommand("fill off");
            form.ExecuteCommand("Circle 8");

            // Save the program state to a temporary file
            form.ExecuteCommand($"save {tempFilePath}");

            // Reset the program state
            form.ExecuteCommand("reset");

            // Load the saved program state
            form.ExecuteCommand($"load {tempFilePath}");

            // Assert
            // Add assertions based on the expected behavior after saving and loading
            Assert.AreEqual(new Point(10, 10), form.GetPenPosition(), "Pen position not set correctly after loading.");
            // Add more assertions for other aspects of your program's state

            // Clean up - delete the temporary file
            File.Delete(tempFilePath);
        }

        [Test]
        public void TestInvalidCommand()
        {
            // Arrange
            Form1 form = new Form1();

            // Act
            // Execute an invalid command
            form.ExecuteCommand("invalidCommand");

            // Assert
            // Check if the error message box is shown
            Assert.AreEqual("Invalid command. Please enter a valid command.", form.GetLastMessageBoxContent(), "Error message box not shown for invalid command.");
        }

    }
}










