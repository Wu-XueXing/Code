using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Happynowyear
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region define the animation border
        private DispatcherTimer bomber = new DispatcherTimer();
        private Dictionary<Bomber, Storyboard> raiseStoryboards = new Dictionary<Bomber, Storyboard>();
        private Dictionary<Explosed, Storyboard> exploseStoryboards = new Dictionary<Explosed, Storyboard>();
        private Dictionary<ExplosedLine, Storyboard> exploseStyleTwoStoryboards = new Dictionary<ExplosedLine, Storyboard>();
        Random random = new Random();
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            #region function and data init
            ScreenHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 200;
            ScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            Ares = (int)(ScreenWidth / 100);

            secondsBetweenBombs = initialSecondsBetweenBombs;
            secondsToRaise = initialSecondsToRaise;
            total = 0;
            explosedStyleOnePart = 66;
            explosedStyleTwoPart = 100;
            explosedStyleThreePart = 8;
            explosedStyleFourPart = 36;

            // Start bomb dropping events.            
            bomber.Interval = TimeSpan.FromSeconds(secondsBetweenBombs);
            bomber.Tick += bomberTimer_Tick;
            bomber.Start();
            #endregion
        }

        #region para to save data
        private double ScreenWidth, ScreenHeight;
        private int Ares;

        private int explosedStyleOnePart;
        private int explosedStyleTwoPart;
        private int explosedStyleThreePart;
        private int explosedStyleFourPart;

        public int wordsShow { get; set; }
        public int wordDispear { get; set; }

        private double initialSecondsBetweenBombs = 1;
        private double initialSecondsToRaise = 1.0;
        private double boomPosition = 300;

        private double boomDuration = 1.2;

        private double secondsBetweenBombs;
        private double secondsToRaise;
        private long total;
        #endregion

        #region the raise animation function
        private void bomberTimer_Tick(object sender, EventArgs e)
        {
            #region set deifferent color
            int sign = random.Next(1, 5);
            Bomber bomb = new Bomber();

            if (sign == 1)
            {
                Application.Current.Resources["BomberMainColor"] = Application.Current.Resources["FireYellow"];
            }
            else if (sign == 2)
            {
                Application.Current.Resources["BomberMainColor"] = Application.Current.Resources["FireBlue"];
            }
            else if (sign == 3)
            {
                Application.Current.Resources["BomberMainColor"] = Application.Current.Resources["FireRed"];
            }
            else
            {
                Application.Current.Resources["BomberMainColor"] = Application.Current.Resources["FireWhite"];
            }
            #endregion

            #region create a bomber and sure will it be explosed in the end
            double sure = random.Next(10);
            if (sure == 0)
            {
                bomb.haveChild = true;
            }
            else { bomb.haveChild = false; }
            canvasBackground.Children.Add(bomb);
            #endregion

            #region set the position to start
            double temp = random.Next(Ares);
            int are = (int)temp * 100;
            temp = random.Next(are, are + 100);

            bomb.positionToLeft = temp;

            Canvas.SetLeft(bomb, temp);
            #endregion

            #region Create the animations
            Storyboard raiseStoryboard = new Storyboard();

            #region the first animation(main action)
            /// random the pisition to end animation
            double temp2 = random.Next(0, 400);
            temp2 = 200 - temp2 + boomPosition;

            bomb.positionToTop = temp2;

            DoubleAnimation raiseAnimation = new DoubleAnimation();
            raiseAnimation.From = ScreenHeight;
            raiseAnimation.To = temp2;
            raiseAnimation.AutoReverse = false;
            raiseAnimation.Duration = TimeSpan.FromSeconds(secondsToRaise);
            raiseStoryboard.Children.Add(raiseAnimation);
            Storyboard.SetTarget(raiseAnimation, bomb);
            Storyboard.SetTargetProperty(raiseAnimation, new PropertyPath("(Canvas.Top)"));
            #endregion

            #region if the bomber didnot explosed, the secound animation is flash
            int animation2 = random.Next(4);
            if (bomb.haveChild)
            {
                DoubleAnimation opcatyAnimation = new DoubleAnimation();
                opcatyAnimation.From = 0.4;
                opcatyAnimation.To = 1;
                opcatyAnimation.AutoReverse = false;
                opcatyAnimation.Duration = TimeSpan.FromSeconds(secondsToRaise);
                opcatyAnimation.EasingFunction = new BackEase { Amplitude = 0.4, EasingMode = EasingMode.EaseOut };
                raiseStoryboard.Children.Add(opcatyAnimation);
                Storyboard.SetTarget(opcatyAnimation, bomb);
                Storyboard.SetTargetProperty(opcatyAnimation, new PropertyPath("(Canvas.Opacity)"));
            }
            else if (animation2 == 2)
            {
                bomb.positionToLeft = temp - 100;

                bomb.RenderTransform = new RotateTransform(350);

                DoubleAnimation wiggleAnimation = new DoubleAnimation();
                wiggleAnimation.From = temp;
                wiggleAnimation.To = temp - 100;
                wiggleAnimation.AutoReverse = false;
                wiggleAnimation.Duration = TimeSpan.FromSeconds(secondsToRaise);
                raiseStoryboard.Children.Add(wiggleAnimation);
                Storyboard.SetTarget(wiggleAnimation, bomb);
                Storyboard.SetTargetProperty(wiggleAnimation, new PropertyPath("(Canvas.Left)"));
            }
            else if (animation2 == 3)
            {
                DoubleAnimation opcatyAnimation = new DoubleAnimation();
                opcatyAnimation.From = 0.3;
                opcatyAnimation.To = 1;
                opcatyAnimation.AutoReverse = false;
                opcatyAnimation.Duration = TimeSpan.FromSeconds(secondsToRaise);
                raiseStoryboard.Children.Add(opcatyAnimation);
                Storyboard.SetTarget(opcatyAnimation, bomb);
                Storyboard.SetTargetProperty(opcatyAnimation, new PropertyPath("(Polygon.Opacity)"));
            }
            #endregion

            #region set the finished action and another action
            raiseStoryboard.Completed += raiseStoryboard_Completed;
            raiseStoryboard.Begin();
            raiseStoryboards.Add(bomb, raiseStoryboard);
            dataText.Text = DateTime.Now.ToString();
            #endregion
            #endregion
        }

        private void raiseStoryboard_Completed(object sender, EventArgs e)
        {
            ClockGroup clockGroup = (ClockGroup)sender;

            #region find the finished the animation and the the data.
            // Get the first animation in the storyboard, and use it to find the bomb that's being animated.
            DoubleAnimation completedAnimation = (DoubleAnimation)clockGroup.Children[0].Timeline;
            Bomber completedBomb = (Bomber)Storyboard.GetTarget(completedAnimation);

            // save the poition of the complated animation.
            double positionToTop = completedBomb.positionToTop;
            double positionToLeft = completedBomb.positionToLeft;
            #endregion

            #region stop the timeline and delete the bomber
            Storyboard storyboard = (Storyboard)clockGroup.Timeline;
            storyboard.Stop();
            raiseStoryboards.Remove(completedBomb);
            canvasBackground.Children.Remove(completedBomb);
            #endregion

            #region calculate the number of bombers.
            if (total == long.MaxValue)
                total = 0;
            total++;
            countText.Text = total.ToString();
            #endregion

            #region start a now animation
            int exploseStyle;
            if (wordsShow > 0)
            {
                exploseStyle = 6;
                wordsShow--;
            }
            else if (completedBomb.haveChild)
            {
                exploseStyle = 5;
            }
            else
            {
                exploseStyle = random.Next(5);
            }
            ExploseAnimation(positionToTop, positionToLeft, exploseStyle);
            #endregion
        }
        #endregion

        #region the explose animation function
        private void ExploseAnimation(double positionToTop, double positionToLeft, int style)
        {
            if (style == 0)
            {
                ExplosedStyleOne(positionToTop, positionToLeft, 1);
                this.mediaCommany.Volume = 1;
                this.mediaCommany.Stop();
                this.mediaCommany.Play();
            }
            else if (style == 1)
            {
                ExplosedStyleOne(positionToTop, positionToLeft, 0.4);
                this.mediaCommany.Volume = 0.4;
                this.mediaCommany.Stop();
                this.mediaCommany.Play();
            }
            else if (style == 2)
            {
                ExplosedStyleTwo(positionToTop, positionToLeft, 1);
                this.mediaCommany.Volume = 1;
                this.mediaCommany.Stop();
                this.mediaCommany.Play();
            }
            else if (style == 3)
            {
                ExplosedStyleTwo(positionToTop, positionToLeft, 0.6);
                this.mediaCommany.Volume = 0.6;
                this.mediaCommany.Stop();
                this.mediaCommany.Play();
            }
            else if (style == 4)
            {
                ExplosedStyleThree(positionToTop, positionToLeft);
                this.mediaCommany.Volume = 1;
                this.mediaCommany.Stop();
                this.mediaCommany.Play();
            }
            else if (style == 5)
            {
                ExplosedStyleFour(positionToTop, positionToLeft, 1);
                this.mediaStyleFour.Volume = 1;
                this.mediaStyleFour.Stop();
                this.mediaStyleFour.Play();
            }
            else if (style == 6)
            {
                ExplosedStyleFive(positionToTop, positionToLeft);
                this.mediaCommany.Volume = 1;
                this.mediaCommany.Stop();
                this.mediaCommany.Play();
            }
        }

        #region explosed style one
        private void ExplosedStyleOne(double positionToTop, double positionToLeft, double scale)
        {
            Storyboard explosedStoryboard = new Storyboard();

            #region define the explose radius
            double MainR = random.Next(100, 200);
            #endregion

            #region create many part
            for (int i = 0; i < explosedStyleOnePart; i++)
            {
                #region calculate the aim position
                double angle = random.Next(5, 356);
                double r;
                int position = random.Next(0, 10);
                if (position <= 2)
                    r = random.Next((int)(MainR / 3), (int)MainR);
                else if (position >= 7)
                    r = random.Next((int)MainR, (int)(MainR * 1.2));
                else
                    r = random.Next((int)MainR - 25, (int)MainR + 25);
                double aimX = positionToLeft - Math.Sin(angle * Math.PI / 180) * r;
                double aimY = positionToTop + Math.Cos(angle * Math.PI / 180) * r;
                #endregion

                #region random the color of the color
                int sign = random.Next(1, 20);
                var mainColor = Application.Current.Resources["BomberMainColor"];
                if (sign == 1)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireYellow"];
                }
                else if (sign == 2)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireBlue"];
                }
                else if (sign == 3)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireRed"];
                }
                else if (sign == 4)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireWhite"];
                }
                else if (sign == 5)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireGreen"];
                }
                else if (sign == 6)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireLightBlue"];
                }
                else if (sign == 7)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FirePink"];
                }
                else if (sign == 8)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireOrange"];
                }
                else
                {
                    Application.Current.Resources["ExplosedMainColor"] = mainColor;
                }
                #endregion

                #region create a now one
                Explosed explosed = new Explosed();
                explosed.IsFlying = true;
                canvasBackground.Children.Add(explosed);
                Canvas.SetLeft(explosed, positionToLeft);
                Canvas.SetTop(explosed, positionToTop);
                #endregion

                #region adjust the size
                explosed.RenderTransform = new ScaleTransform(scale, scale);
                #endregion

                #region define the animation
                DoubleAnimation xAnimation = new DoubleAnimation();
                xAnimation.From = positionToLeft;
                xAnimation.To = aimX;
                xAnimation.AutoReverse = false;
                xAnimation.Duration = TimeSpan.FromSeconds(boomDuration);
                //xAnimation.EasingFunction = new BackEase { Amplitude = 0.1, EasingMode = EasingMode.EaseOut };
                explosedStoryboard.Children.Add(xAnimation);
                Storyboard.SetTarget(xAnimation, explosed);
                Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(Canvas.Left)"));
                #endregion

                #region define the secound animation
                DoubleAnimation yAnimaiton = new DoubleAnimation();
                yAnimaiton.From = positionToTop;
                yAnimaiton.To = aimY;
                yAnimaiton.AutoReverse = false;
                yAnimaiton.Duration = TimeSpan.FromSeconds(boomDuration);
                //yAnimaiton.EasingFunction = new BackEase { Amplitude = 0.1, EasingMode = EasingMode.EaseOut };
                explosedStoryboard.Children.Add(yAnimaiton);
                Storyboard.SetTarget(yAnimaiton, explosed);
                Storyboard.SetTargetProperty(yAnimaiton, new PropertyPath("(Canvas.Top)"));
                #endregion

                #region the third animation
                DoubleAnimation scaleAnimation = new DoubleAnimation();
                scaleAnimation.From = 0.1;
                scaleAnimation.To = 0.6;
                scaleAnimation.Duration = TimeSpan.FromSeconds(boomDuration);
                scaleAnimation.AutoReverse = false;
                explosed.RenderTransform = new ScaleTransform();
                explosed.RenderTransformOrigin = new Point(0.5, 0.5);
                Storyboard.SetTarget(scaleAnimation, explosed);
                Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath("(RenderTransform.ScaleX)"));
                Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath("(RenderTransform.ScaleY)"));
                #endregion

                exploseStoryboards.Add(explosed, explosedStoryboard);
            }
            #endregion

            #region set the finished
            explosedStoryboard.Completed += exploseStyleOne_Completed;
            explosedStoryboard.Begin();
            #endregion
        }
        private void exploseStyleOne_Completed(object sender, EventArgs e)
        {
            ClockGroup clockGroup = (ClockGroup)sender;

            #region find the finished the animation and the the data; stop the timeline and delete the explosed
            for (int i = 0; i < explosedStyleOnePart; i++)
            {
                // Get the first animation in the storyboard, and use it to find the bomb that's being animated.
                DoubleAnimation completedAnimation = (DoubleAnimation)clockGroup.Children[2 * i].Timeline;
                Explosed completedExplosed = (Explosed)Storyboard.GetTarget(completedAnimation);

                Storyboard storyboard = (Storyboard)clockGroup.Timeline;
                storyboard.Stop();
                exploseStoryboards.Remove(completedExplosed);
                canvasBackground.Children.Remove(completedExplosed);
            }
            #endregion
        }
        #endregion

        #region explosed style two
        private void ExplosedStyleTwo(double positionToTop, double positionToLeft, double scale)
        {
            Storyboard explosedStoryboard = new Storyboard();

            #region define the explose radius
            double MainR = random.Next(100, 200);
            #endregion

            #region create many part
            for (int i = 0; i < explosedStyleTwoPart; i++)
            {
                #region calculate the aim position
                double angle = random.Next(5, 356);
                double r;
                int position = random.Next(0, 10);
                if (position <= 2)
                    r = random.Next((int)(MainR / 3), (int)MainR);
                else if (position >= 7)
                    r = random.Next((int)MainR, (int)(MainR * 1.2));
                else
                    r = random.Next((int)MainR - 25, (int)MainR + 25);
                double aimX = positionToLeft - Math.Sin(angle * Math.PI / 180) * r;
                double aimY = positionToTop + Math.Cos(angle * Math.PI / 180) * r;
                #endregion

                #region random the color of the color
                int sign = random.Next(1, 20);
                var mainColor = Application.Current.Resources["BomberMainColor"];
                if (sign == 1)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireYellow"];
                }
                else if (sign == 2)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireBlue"];
                }
                else if (sign == 3)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireRed"];
                }
                else if (sign == 4)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireWhite"];
                }
                else if (sign == 5)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireGreen"];
                }
                else if (sign == 6)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireLightBlue"];
                }
                else if (sign == 7)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FirePink"];
                }
                else if (sign == 8)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireOrange"];
                }
                else
                {
                    Application.Current.Resources["ExplosedMainColor"] = mainColor;
                }
                #endregion

                #region create a now one
                ExplosedLine explosed = new ExplosedLine();
                canvasBackground.Children.Add(explosed);
                Canvas.SetLeft(explosed, positionToLeft);
                Canvas.SetTop(explosed, positionToTop);
                #endregion

                #region adjust the size
                explosed.RenderTransform = new ScaleTransform(scale, scale);
                #endregion

                #region rotate the part
                //explosed.RenderTransform = new RotateTransform { Angle = angle };
                explosed.LayoutTransform = new RotateTransform(angle);
                #endregion

                #region define the animation
                DoubleAnimation xAnimation = new DoubleAnimation();
                xAnimation.From = positionToLeft;
                xAnimation.To = aimX;
                xAnimation.AutoReverse = false;
                xAnimation.Duration = TimeSpan.FromSeconds(boomDuration);
                //xAnimation.EasingFunction = new BackEase { Amplitude = 0.1, EasingMode = EasingMode.EaseOut };
                explosedStoryboard.Children.Add(xAnimation);
                Storyboard.SetTarget(xAnimation, explosed);
                Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(Canvas.Left)"));
                #endregion

                #region define the secound animation
                DoubleAnimation yAnimaiton = new DoubleAnimation();
                yAnimaiton.From = positionToTop;
                yAnimaiton.To = aimY;
                yAnimaiton.AutoReverse = false;
                yAnimaiton.Duration = TimeSpan.FromSeconds(boomDuration);
                //yAnimaiton.EasingFunction = new BackEase { Amplitude = 0.1, EasingMode = EasingMode.EaseOut };
                explosedStoryboard.Children.Add(yAnimaiton);
                Storyboard.SetTarget(yAnimaiton, explosed);
                Storyboard.SetTargetProperty(yAnimaiton, new PropertyPath("(Canvas.Top)"));
                #endregion

                #region the third animation
                DoubleAnimation opcatyAnimation = new DoubleAnimation();
                opcatyAnimation.From = 0;
                opcatyAnimation.To = 1;
                opcatyAnimation.AutoReverse = false;
                opcatyAnimation.Duration = TimeSpan.FromSeconds(boomDuration);
                explosedStoryboard.Children.Add(opcatyAnimation);
                Storyboard.SetTarget(opcatyAnimation, explosed);
                Storyboard.SetTargetProperty(opcatyAnimation, new PropertyPath("(Polygon.Opacity)"));
                #endregion

                exploseStyleTwoStoryboards.Add(explosed, explosedStoryboard);
            }
            #endregion

            #region set the finished
            explosedStoryboard.Completed += exploseStyleTwo_Completed;
            explosedStoryboard.Begin();
            #endregion
        }
        private void exploseStyleTwo_Completed(object sender, EventArgs e)
        {
            ClockGroup clockGroup = (ClockGroup)sender;

            #region find the finished the animation and the the data; stop the timeline and delete the explosed
            for (int i = 0; i < explosedStyleTwoPart; i++)
            {
                // Get the first animation in the storyboard, and use it to find the bomb that's being animated.
                DoubleAnimation completedAnimation = (DoubleAnimation)clockGroup.Children[3 * i].Timeline;
                ExplosedLine completedExplosed = (ExplosedLine)Storyboard.GetTarget(completedAnimation);

                Storyboard storyboard = (Storyboard)clockGroup.Timeline;
                storyboard.Stop();
                exploseStyleTwoStoryboards.Remove(completedExplosed);
                canvasBackground.Children.Remove(completedExplosed);
            }
            #endregion
        }
        #endregion

        #region explosed style three
        private void ExplosedStyleThree(double positionToTop, double positionToLeft)
        {
            Storyboard explosedStoryboard = new Storyboard();

            #region define the explose radius
            double MainR = random.Next(100, 200);
            #endregion

            #region create foru main part
            for (int j = 0; j < 4; j++)
            {
                double angeleMain = 60 * (j + 1) + random.Next(0, 60);

                #region create many part
                for (int i = 0; i <= explosedStyleThreePart; i++)
                {
                    #region calculate the aim position
                    double angle;
                    double r;
                    int position = random.Next(0, 10);
                    if (i == 0)
                    {
                        angle = angeleMain;
                        r = MainR;
                    }
                    else
                    {
                        r = random.Next((int)(MainR / 3), (int)MainR);
                        angle = angeleMain + 10 - random.Next(0, 20);
                    }
                    double aimX = positionToLeft - Math.Sin(angle * Math.PI / 180) * r;
                    double aimY = positionToTop + Math.Cos(angle * Math.PI / 180) * r;
                    #endregion

                    #region random the color of the color
                    int sign = random.Next(1, 20);
                    var mainColor = Application.Current.Resources["BomberMainColor"];

                    if (i == 0)
                    {
                        Application.Current.Resources["ExplosedMainColor"] = mainColor;
                    }
                    else if (sign == 1)
                    {
                        Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireYellow"];
                    }
                    else if (sign == 2)
                    {
                        Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireBlue"];
                    }
                    else if (sign == 3)
                    {
                        Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireRed"];
                    }
                    else if (sign == 4)
                    {
                        Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireWhite"];
                    }
                    else if (sign == 5)
                    {
                        Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireGreen"];
                    }
                    else if (sign == 6)
                    {
                        Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireLightBlue"];
                    }
                    else if (sign == 7)
                    {
                        Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FirePink"];
                    }
                    else if (sign == 8)
                    {
                        Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireOrange"];
                    }
                    else
                    {
                        Application.Current.Resources["ExplosedMainColor"] = mainColor;
                    }
                    #endregion

                    #region create a now one
                    Explosed explosed = new Explosed();
                    explosed.IsFlying = true;
                    canvasBackground.Children.Add(explosed);
                    Canvas.SetLeft(explosed, positionToLeft);
                    Canvas.SetTop(explosed, positionToTop);
                    #endregion

                    #region adjust the size
                    if (i == 0)
                    {
                        explosed.RenderTransform = new ScaleTransform(1.6, 1.6);
                    }
                    else
                    {
                        explosed.RenderTransform = new ScaleTransform(0.8, 0.8);
                    }
                    #endregion

                    #region define the animation
                    DoubleAnimation xAnimation = new DoubleAnimation();
                    xAnimation.From = positionToLeft;
                    xAnimation.To = aimX;
                    xAnimation.AutoReverse = false;
                    xAnimation.Duration = TimeSpan.FromSeconds(boomDuration);
                    //xAnimation.EasingFunction = new BackEase { Amplitude = 0.1, EasingMode = EasingMode.EaseOut };
                    explosedStoryboard.Children.Add(xAnimation);
                    Storyboard.SetTarget(xAnimation, explosed);
                    Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(Canvas.Left)"));
                    #endregion

                    #region define the secound animation
                    DoubleAnimation yAnimaiton = new DoubleAnimation();
                    yAnimaiton.From = positionToTop;
                    yAnimaiton.To = aimY;
                    yAnimaiton.AutoReverse = false;
                    yAnimaiton.Duration = TimeSpan.FromSeconds(boomDuration);
                    //yAnimaiton.EasingFunction = new BackEase { Amplitude = 0.1, EasingMode = EasingMode.EaseOut };
                    explosedStoryboard.Children.Add(yAnimaiton);
                    Storyboard.SetTarget(yAnimaiton, explosed);
                    Storyboard.SetTargetProperty(yAnimaiton, new PropertyPath("(Canvas.Top)"));
                    #endregion

                    exploseStoryboards.Add(explosed, explosedStoryboard);
                }
                #endregion
            }
            #endregion

            #region set the finished
            explosedStoryboard.Completed += exploseStyleThree_Completed;
            explosedStoryboard.Begin();
            #endregion
        }
        private void exploseStyleThree_Completed(object sender, EventArgs e)
        {
            ClockGroup clockGroup = (ClockGroup)sender;

            #region find the finished the animation and the the data; stop the timeline and delete the explosed
            for (int i = 0; i < (explosedStyleThreePart + 1) * 4; i++)
            {
                // Get the first animation in the storyboard, and use it to find the bomb that's being animated.
                DoubleAnimation completedAnimation = (DoubleAnimation)clockGroup.Children[2 * i].Timeline;
                Explosed completedExplosed = (Explosed)Storyboard.GetTarget(completedAnimation);

                Storyboard storyboard = (Storyboard)clockGroup.Timeline;
                storyboard.Stop();
                exploseStoryboards.Remove(completedExplosed);
                canvasBackground.Children.Remove(completedExplosed);
            }
            #endregion
        }
        #endregion

        #region explosed style four
        private void ExplosedStyleFour(double positionToTop, double positionToLeft, double scale)
        {
            Storyboard explosedStoryboard = new Storyboard();

            #region create many part
            for (int i = 0; i < explosedStyleFourPart; i++)
            {
                #region calculate the aim position
                double angle = 8 - random.Next(0, 17);
                int position = random.Next((int)(positionToTop + 50), (int)(ScreenHeight - 50));
                //positionToLeft + 10 - random.Next(0,21)
                double aimX = positionToLeft - angle;
                double aimY = Math.Cos(angle * Math.PI / 180) * position;
                #endregion

                #region random the color of the color
                int sign = random.Next(1, 20);
                var mainColor = Application.Current.Resources["BomberMainColor"];
                if (sign == 1)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireYellow"];
                }
                else if (sign == 2)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireBlue"];
                }
                else if (sign == 3)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireRed"];
                }
                else if (sign == 4)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireWhite"];
                }
                else if (sign == 5)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireGreen"];
                }
                else if (sign == 6)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireLightBlue"];
                }
                else if (sign == 7)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FirePink"];
                }
                else if (sign == 8)
                {
                    Application.Current.Resources["ExplosedMainColor"] = Application.Current.Resources["FireOrange"];
                }
                else
                {
                    Application.Current.Resources["ExplosedMainColor"] = mainColor;
                }
                #endregion

                #region create a now one
                Explosed explosed = new Explosed();
                canvasBackground.Children.Add(explosed);
                Canvas.SetLeft(explosed, positionToLeft);
                Canvas.SetTop(explosed, aimY + 20);
                #endregion

                #region adjust the size
                explosed.RenderTransform = new ScaleTransform(scale, scale);
                #endregion

                #region rotate the part
                //explosed.RenderTransform = new RotateTransform { Angle = angle };
                //explosed.RenderTransformOrigin = new Point(0.5, 0.5);
                //explosed.LayoutTransform = new RotateTransform(180 - 2 * angle);
                #endregion

                #region define the animation
                DoubleAnimation xAnimation = new DoubleAnimation();
                xAnimation.From = positionToLeft;
                xAnimation.To = aimX;
                xAnimation.AutoReverse = false;
                xAnimation.BeginTime = TimeSpan.FromSeconds(0.3);
                xAnimation.Duration = TimeSpan.FromSeconds(boomDuration - 0.3);
                //xAnimation.EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseIn };
                explosedStoryboard.Children.Add(xAnimation);
                Storyboard.SetTarget(xAnimation, explosed);
                Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(Canvas.Left)"));
                #endregion

                #region define the secound animation
                DoubleAnimation yAnimaiton = new DoubleAnimation();
                yAnimaiton.From = aimY + 50;
                yAnimaiton.To = aimY;
                yAnimaiton.AutoReverse = false;
                yAnimaiton.Duration = TimeSpan.FromSeconds(boomDuration);
                //yAnimaiton.EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut };
                explosedStoryboard.Children.Add(yAnimaiton);
                Storyboard.SetTarget(yAnimaiton, explosed);
                Storyboard.SetTargetProperty(yAnimaiton, new PropertyPath("(Canvas.Top)"));
                #endregion

                #region the third animation
                DoubleAnimation opcatyAnimation = new DoubleAnimation();
                opcatyAnimation.From = 1;
                opcatyAnimation.To = 0.4;
                opcatyAnimation.AutoReverse = true;
                //opcatyAnimation.BeginTime = TimeSpan.FromSeconds(1);
                opcatyAnimation.Duration = TimeSpan.FromSeconds(boomDuration / 4);
                //opcatyAnimation.EasingFunction = new BackEase { Amplitude = 0.4, EasingMode = EasingMode.EaseOut };
                explosedStoryboard.Children.Add(opcatyAnimation);
                Storyboard.SetTarget(opcatyAnimation, explosed);
                Storyboard.SetTargetProperty(opcatyAnimation, new PropertyPath("(Canvas.Opacity)"));
                #endregion

                #region define the fourth animation
                //DoubleAnimation roateAnimation = new DoubleAnimation();
                //explosed.RenderTransform = new RotateTransform();
                //explosed.RenderTransformOrigin = new Point(0.5, 0.5);
                //roateAnimation.From = 180;
                //roateAnimation.To = 180 - 2 * angle;
                //roateAnimation.Duration = TimeSpan.FromSeconds(boomDuration);
                //roateAnimation.AutoReverse = false;
                //explosedStoryboard.Children.Add(roateAnimation);
                //Storyboard.SetTarget(roateAnimation, explosed);
                //Storyboard.SetTargetProperty(roateAnimation, new PropertyPath("(RenderTransform.Angle)"));
                #endregion

                exploseStoryboards.Add(explosed, explosedStoryboard);
            }
            #endregion

            #region set the finished
            explosedStoryboard.Completed += exploseStyleFour_Completed;
            explosedStoryboard.Begin();
            #endregion
        }
        private void exploseStyleFour_Completed(object sender, EventArgs e)
        {
            ClockGroup clockGroup = (ClockGroup)sender;

            #region find the finished the animation and the the data; stop the timeline and delete the explosed
            for (int i = 0; i < explosedStyleFourPart; i++)
            {
                // Get the first animation in the storyboard, and use it to find the bomb that's being animated.
                DoubleAnimation completedAnimation = (DoubleAnimation)clockGroup.Children[3 * i].Timeline;
                Explosed completedExplosed = (Explosed)Storyboard.GetTarget(completedAnimation);

                Storyboard storyboard = (Storyboard)clockGroup.Timeline;
                storyboard.Stop();
                exploseStoryboards.Remove(completedExplosed);
                canvasBackground.Children.Remove(completedExplosed);
            }
            #endregion
        }
        #endregion

        #region explosed style: show happy new year.
        private void ExplosedStyleFive(double positionToTop, double positionToLeft)
        {
            Storyboard textStoryboard = new Storyboard();

            #region random the scale size
            double scale = (double)(random.Next(70, 150)) / 100;
            #endregion

            #region random the color of the color
            int sign = random.Next(1, 20);
            var mainColor = Application.Current.Resources["BomberMainColor"];
            if (sign == 1)
            {
                Application.Current.Resources["FlashColor"] = Application.Current.Resources["FireYellow"];
            }
            else if (sign == 2)
            {
                Application.Current.Resources["FlashColor"] = Application.Current.Resources["FireBlue"];
            }
            else if (sign == 3)
            {
                Application.Current.Resources["FlashColor"] = Application.Current.Resources["FireRed"];
            }
            else if (sign == 4)
            {
                Application.Current.Resources["FlashColor"] = Application.Current.Resources["FireWhite"];
            }
            else if (sign == 5)
            {
                Application.Current.Resources["FlashColor"] = Application.Current.Resources["FireGreen"];
            }
            else if (sign == 6)
            {
                Application.Current.Resources["FlashColor"] = Application.Current.Resources["FireLightBlue"];
            }
            else if (sign == 7)
            {
                Application.Current.Resources["FlashColor"] = Application.Current.Resources["FirePink"];
            }
            else if (sign == 8)
            {
                Application.Current.Resources["FlashColor"] = Application.Current.Resources["FireOrange"];
            }
            else
            {
                Application.Current.Resources["FlashColor"] = mainColor;
            }
            #endregion

            #region define the Animation
            DoubleAnimation scalexAnimation = new DoubleAnimation();
            scalexAnimation.From = 0.3;
            scalexAnimation.To = scale;
            scalexAnimation.AutoReverse = false;
            scalexAnimation.Duration = TimeSpan.FromSeconds(boomDuration);

            DoubleAnimation scaleyAnimation = new DoubleAnimation();
            scaleyAnimation.From = 0.3;
            scaleyAnimation.To = scale;
            scaleyAnimation.AutoReverse = false;
            scaleyAnimation.Duration = TimeSpan.FromSeconds(boomDuration);

            DoubleAnimation upAnimation = new DoubleAnimation();
            upAnimation.From = positionToTop;
            upAnimation.To = positionToTop - 150;
            upAnimation.AutoReverse = false;
            scaleyAnimation.Duration = TimeSpan.FromSeconds(boomDuration);
            #endregion

            #region create the different word
            if (wordsShow == 3)
            {
                TextNew word = new TextNew();
                canvasBackground.Children.Add(word);
                Canvas.SetTop(word, positionToTop);
                Canvas.SetLeft(word, positionToLeft);

                word.RenderTransform = new ScaleTransform();
                word.RenderTransformOrigin = new Point(0.5, 0.5);

                Storyboard.SetTarget(scalexAnimation, word);
                Storyboard.SetTarget(scaleyAnimation, word);
                Storyboard.SetTarget(upAnimation, word);
            }
            else if (wordsShow == 2)
            {
                TextYear word = new TextYear();
                canvasBackground.Children.Add(word);
                Canvas.SetTop(word, positionToTop);
                Canvas.SetLeft(word, positionToLeft);

                word.RenderTransform = new ScaleTransform();
                word.RenderTransformOrigin = new Point(0.5, 0.5);

                Storyboard.SetTarget(scalexAnimation, word);
                Storyboard.SetTarget(scaleyAnimation, word);
                Storyboard.SetTarget(upAnimation, word);
            }
            else if (wordsShow == 1)
            {
                TextFast word = new TextFast();
                canvasBackground.Children.Add(word);
                Canvas.SetTop(word, positionToTop);
                Canvas.SetLeft(word, positionToLeft);

                word.RenderTransform = new ScaleTransform();
                word.RenderTransformOrigin = new Point(0.5, 0.5);

                Storyboard.SetTarget(scalexAnimation, word);
                Storyboard.SetTarget(scaleyAnimation, word);
                Storyboard.SetTarget(upAnimation, word);
            }
            else
            {
                TextHappy word = new TextHappy();
                canvasBackground.Children.Add(word);
                Canvas.SetTop(word, positionToTop);
                Canvas.SetLeft(word, positionToLeft);

                word.RenderTransform = new ScaleTransform();
                word.RenderTransformOrigin = new Point(0.5, 0.5);

                Storyboard.SetTarget(scalexAnimation, word);
                Storyboard.SetTarget(scaleyAnimation, word);
                Storyboard.SetTarget(upAnimation, word);
            }
            #endregion

            #region finished the animatioin
            Storyboard.SetTargetProperty(scalexAnimation, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(scaleyAnimation, new PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTargetProperty(upAnimation, new PropertyPath("(Canvas.Top)"));

            textStoryboard.Children.Add(scalexAnimation);
            textStoryboard.Children.Add(scaleyAnimation);
            textStoryboard.Children.Add(upAnimation);
            textStoryboard.Completed += exploseStyleFive_Completed;
            textStoryboard.Begin();
            #endregion
        }
        private void exploseStyleFive_Completed(object sender, EventArgs e)
        {
            ClockGroup clockGroup = (ClockGroup)sender;

            DoubleAnimation completedAnimation = (DoubleAnimation)clockGroup.Children[0].Timeline;

            if (wordDispear == 4)
            {
                TextNew completedBomb = (TextNew)Storyboard.GetTarget(completedAnimation);
                Storyboard storyboard = (Storyboard)clockGroup.Timeline;
                storyboard.Stop();
                canvasBackground.Children.Remove(completedBomb);
            }
            else if (wordDispear == 3)
            {
                TextYear completedBomb = (TextYear)Storyboard.GetTarget(completedAnimation);
                Storyboard storyboard = (Storyboard)clockGroup.Timeline;
                storyboard.Stop();
                canvasBackground.Children.Remove(completedBomb);
            }
            else if (wordDispear == 2)
            {
                TextFast completedBomb = (TextFast)Storyboard.GetTarget(completedAnimation);
                Storyboard storyboard = (Storyboard)clockGroup.Timeline;
                storyboard.Stop();
                canvasBackground.Children.Remove(completedBomb);
            }
            else if (wordDispear == 1)
            {
                TextHappy completedBomb = (TextHappy)Storyboard.GetTarget(completedAnimation);
                Storyboard storyboard = (Storyboard)clockGroup.Timeline;
                storyboard.Stop();
                canvasBackground.Children.Remove(completedBomb);
            }
            wordDispear--;
        }
        #endregion

        #endregion

        /// define the exit function button.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void mediaCommany_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message, "Error message:");
        }

        private void bgmbutton_Click(object sender, RoutedEventArgs e)
        {
            if (wordDispear == 0)
            {
                wordsShow = 4;
                wordDispear = wordsShow;
            }
        }
    }
}
