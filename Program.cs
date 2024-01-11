using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

class Program {
    static void Main(string[] args) {
        Console.Clear();
        renderWalls();

        // delay between each frame in milliseconds
        const int delay = 75;

        Snake snake = new Snake('■', ConsoleColor.Green);
        snake.xpos = 5;
        snake.ypos = 5;
        snake.direction = Direction.Down;

        Apple apple = new Apple('■', ConsoleColor.Red);
        apple.setPosition();

        List<Body> snakeBody = new List<Body>();

        while (true){
            if (Console.KeyAvailable) {
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch(key.Key) {
                    case ConsoleKey.W:
                        if (snake.direction != Direction.Down) {
                            snake.direction = Direction.Up;
                        }
                        break;
                    case ConsoleKey.S:
                        if (snake.direction != Direction.Up) {
                            snake.direction = Direction.Down;
                        }
                        break;
                    case ConsoleKey.D:
                        if (snake.direction != Direction.Left) {
                            snake.direction = Direction.Right;
                        }
                        break;
                    case ConsoleKey.A:
                        if (snake.direction != Direction.Right) {
                            snake.direction = Direction.Left;
                        }
                        break;
                }
            }

            // derender everything so it can be re-rendered later
            snake.derender();
            apple.derender();
            foreach (Body piece in snakeBody) {
                piece.derender();
            }

            // add a new piece to the body 
            Body newPiece = new Body('■', ConsoleColor.Green);
            newPiece.xpos = snake.xpos;
            newPiece.ypos = snake.ypos;
            snakeBody.Add(newPiece);

            // Move the snake according to its direction
            switch (snake.direction) {
                case Direction.Up:
                    snake.ypos -= 1;
                    break;
                case Direction.Down:
                    snake.ypos += 1;
                    break;
                case Direction.Right:
                    snake.xpos += 1;
                    break;
                case Direction.Left:
                    snake.xpos -= 1;
                    break;
            }

            // remove the last piece of the body, unless the snake touched the apple
            if (snake.collision(apple)) {
                apple.setPosition();
            } else {
                snakeBody.RemoveAt(0);
            }

            // check if the snake ran into any walls
            if (snake.xpos <= 1 || snake.xpos >= (Console.WindowWidth- 1) || snake.ypos <= 0 || snake.ypos >= (Console.WindowHeight - 1)) {
                break;
            }

            // render all objects 
            snake.render();
            apple.render();
            // render each body piece AND check if the head collided with any of the body parts
            foreach (Body piece in snakeBody) {
                if (snake.collision(piece)) {
                    // the notorious goto
                    // this just jumps out of the while loop
                    goto End;
                }
                piece.render();
            }
            // keep the cursor up in the corner
            Console.SetCursorPosition(0, 0);
            System.Threading.Thread.Sleep(delay);
        }
        End:
            Console.Clear();
            Console.WriteLine("Final Score: " + snakeBody.Count);
    }
    public static void renderWalls() {
        for (int i = 0; i < Console.WindowWidth; i++) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(i, 0);
            Console.Write('I');
            Console.SetCursorPosition(i, Console.WindowHeight);
            Console.Write('I');
        }
        for (int i = 0; i < Console.WindowHeight; i++) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(0, i);
            Console.Write('I');
            Console.SetCursorPosition(Console.WindowWidth, i);
            Console.Write('I');
        }
    }
}

enum Direction {
    Up,
    Down,
    Left,
    Right
}

// Base class that handles rendering/derendering of all on-screen objects
class Object {
    private char character;
    private ConsoleColor color;
    public Object(char c, ConsoleColor clr) {
        character = c;
        color = clr;
    }
    public int xpos { get; set; }

    public int ypos { get; set; }
    
    public void derender() {
        Console.SetCursorPosition(xpos, ypos);
        Console.Write(" ");
    }

    public void render() {
        Console.ForegroundColor = color;
        Console.SetCursorPosition(xpos, ypos);
        Console.Write(character);
        Console.ForegroundColor = ConsoleColor.Black;
    }
    public bool collision(Object other) {
        return xpos == other.xpos && ypos == other.ypos;
    }
}

class Snake : Object {
    public Snake (char c, ConsoleColor clr) : base(c, clr) {}
    public Direction direction { get; set; }

}

class Apple : Object {
    public Apple (char c, ConsoleColor clr) : base(c, clr) {}
    public void setPosition() {
        Random random = new Random();
        xpos = random.Next(1, Console.WindowWidth - 1);
        ypos = random.Next(1, Console.WindowHeight - 1);
    }
}

class Body : Object {
    public Body (char c, ConsoleColor clr) : base(c, clr) {}
}