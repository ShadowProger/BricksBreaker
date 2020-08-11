using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Manybits
{
    // Состояние поля
    public enum FieldState { BeforeStart, PlayerTurn, Idle, BallStartMove, BlocksCreate, BlocksMove, GameOver, Pause }

    public class Field : MonoBehaviour
    {
        #region PUBLIC FIELDS

        public GameManager gameManager;

        public GameObject LeftBorder;
        public GameObject RightBorder;
        public GameObject TopBorder;
        public GameObject BottomBorder;

        public GameObject blockPrefab;
        public Transform blockParent;

        public GameObject ballPrefab;

        public Transform ballParent;

        public Brick brickPrefab;
        public BallBonus ballBonusPrefab;
        public StarBonus starBonusPrefab;
        public ShieldBonus shieldBonusPrefab;
        public DoubleBallBonus doubleBallBonusPrefab;
        public HorizontalLazer horizontalLazerPrefab;
        public VerticalLazer verticalLazerPrefab;
        public DoubleLazer doubleLazerPrefab;

        public Animator checkpointAnimator;

        public GameObject ballStart;
        public GameObject ballNextStart;

        public bool isGameOver;

        public Text ballCountText;

        public LineRenderer aimLine;
        public LineRenderer helpLine;

        public Transform particlesParent;

        //test
        //public Text ballText;

        #endregion

        #region PRIVATE FIELDS

        private int width;
        private int height;

        private Rect openRect; // Область, в которой размещается поле
        private Rect borders; // Границы поля

        private float cellSize;

        private Pool<Ball> ballPool;

        private List<Block> blocks = new List<Block>();
        private List<Ball> balls = new List<Ball>();

        private Queue<List<BlockType>> lines;

        private int ballsCount;
        private int ballsCountForTurn;
        private int ballStopCount;
        private int lineNumber;

        private float fieldScale;

        private Vector2 nextStart;

        private float ballStartY;
        private float ballStartXMin;
        private float ballStartXMax;

        private Coroutine throwBallsCoroutine;

        private bool isFirstBallPositionSaved = false;
        private bool isBallStartOnPosition = false;

        private bool canTurn = false;

        private List<Block> newBlocks = new List<Block>();
        private int newBlocksCount;

        private int brickPointCount;
        private bool isAllBricksBroken;

        private bool isWin;

        private bool IsTurnEnded = true;

        private Generator generator;

        private static FieldState fieldState;
        private static FieldState previousState;

        private bool isAlreadyAberted;
        private bool isTimerStarted;
        private bool isFastForwardButtonShow;
        private float fastForwardTimer;
        private const float fastForwardDelay = 10f;

        private int score;

        #endregion

        #region PROPERTIES

        public int Score
        {
            get => score;
            set
            {
                score = value;
                if (onScoreChange != null)
                {
                    onScoreChange.Invoke(score);
                }
            }
        }

        public static FieldState FieldState
        {
            get => fieldState;
        }

        public Vector2 BallPosition
        {
            get => ballStart.transform.position;
        }

        public int BallsCount
        {
            get => ballsCount;
            set => ballsCount = value;
        }

        public int BallStopCount
        {
            get => ballStopCount;
            set
            {
                ballStopCount = value;
            }
        }

        public bool CanTurn
        {
            get => canTurn;
            set => canTurn = value;
        }
        
        public bool IsFirstBallPositionSaved
        {
            get => isFirstBallPositionSaved;
            set => isFirstBallPositionSaved = value;
        }

        public int BrickPointCount
        {
            get => brickPointCount;
            set => brickPointCount = value;
        }

        public int Height
        {
            get => height;
        }

        public int Width
        {
            get => width;
        }
        
        public Transform ParticlesParent
        {
            get => particlesParent;
        }

        public ScoreChangeEvent onScoreChange { get; set; }

        public LineNumberChangeEvent onLineNumberChange { get; set; }

        public RemainLinesChangeEvent onRemainLinesChange { get; set; }

        #endregion

        #region MONOBEHAVIOUR METHODS

        void Update()
        {
            switch (fieldState)
            {
                case FieldState.BeforeStart:
                    break;

                case FieldState.PlayerTurn:
                    if (brickPointCount == 0)
                    {
                        if (isAllBricksBroken == false)
                        {
                            isAllBricksBroken = true;
                            if (GameManager.gameMode == GameMode.Infinity)
                            {
                                if (lineNumber > gameManager.gameInfo.checkpoint)
                                    checkpointAnimator.SetTrigger("Appear");

                                gameManager.gameInfo.checkpoint = lineNumber;
                            }
                            else
                            {
                                if (lines.Count == 0)
                                {
                                    isWin = true;
                                }
                            }
                        }
                    }
                    if (GameManager.gameMode == GameMode.DailyChallenge && isWin)
                    {
                        SetState(FieldState.GameOver);

                        // Сохранить DailyChallengeProgress
                        gameManager.AddCompliteDay(gameManager.selectedDay);
                        gameManager.SaveChallengeProgress();

                        ScreenManager.Instance.levelCompleteInterface.Open();
                    }
                    break;

                case FieldState.Idle:
                    if (isTimerStarted)
                    {
                        fastForwardTimer -= Time.deltaTime;
                        if (fastForwardTimer <= 0f && !isFastForwardButtonShow)
                        {
                            isFastForwardButtonShow = true;
                            ScreenManager.Instance.gameInterface.ShowFastForwardButton(true);
                        }
                    }

                    if (IsTurnEnded && balls.Count == 0)
                    {
                        // удалить бонусы, которые должны быть удалены
                        for (int i = 0; i < blocks.Count;)
                        {
                            if (blocks[i].NeedDelete())
                            {
                                RemoveBlock(blocks[i]);
                            }
                            else
                                i++;
                        }

                        Time.timeScale = 1f;
                        ScreenManager.Instance.gameInterface.ShowFastForwardButton(false);
                        ScreenManager.Instance.gameInterface.ShowFastForwardButton(false);

                        StartCoroutine(MoveBallStart(0.2f));
                        SetState(FieldState.BallStartMove);
                    }

                    if (brickPointCount == 0)
                    {
                        if (isAllBricksBroken == false)
                        {
                            isAllBricksBroken = true;
                            if (GameManager.gameMode == GameMode.Infinity)
                            {
                                if (lineNumber > gameManager.gameInfo.checkpoint)
                                    checkpointAnimator.SetTrigger("Appear");

                                gameManager.gameInfo.checkpoint = lineNumber;
                            }
                            else
                            {
                                if (lines.Count == 0)
                                {
                                    isWin = true;
                                }
                            }
                        }
                    }

                    break;

                case FieldState.BallStartMove:
                    if (isBallStartOnPosition)
                    {
                        ballNextStart.SetActive(false);
                        if (GameManager.gameMode == GameMode.DailyChallenge && isWin)
                        {
                            SetState(FieldState.GameOver);

                            // Сохранить DailyChallengeProgress
                            gameManager.AddCompliteDay(gameManager.selectedDay);
                            gameManager.SaveChallengeProgress();

                            ScreenManager.Instance.levelCompleteInterface.Open();
                            break;
                        }

                        CreateNewLine();
                        SetState(FieldState.BlocksCreate);
                        canTurn = false;
                    }
                    break;

                case FieldState.BlocksCreate:
                    if (newBlocksCount >= newBlocks.Count)
                    {
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            blocks[i].MoveDown(cellSize * fieldScale, 0.2f);
                        }

                        SetState(FieldState.BlocksMove);
                        canTurn = false;
                    }
                    break;

                case FieldState.BlocksMove:
                    if (canTurn)
                    {
                        if (GameManager.gameMode == GameMode.Infinity)
                        {
                            SaveField();
                        }

                        if (isGameOver)
                        {
                            SetState(FieldState.GameOver);
                            ScreenManager.Instance.continueInterface.Open();
                        }
                        else
                        {
                            NextTurn();
                        }
                    }
                    break;

                case FieldState.GameOver:
                    break;
            }

            aimLine.material.mainTextureOffset -= new Vector2(0.06f, 0f);
        }

        #endregion

        #region METHODS

        public void PreInit()
        {
            Debug.Log("[Field] PreInit");
            ballPool = new Pool<Ball>(ballPrefab, 50);
            onScoreChange = new ScoreChangeEvent();
            onLineNumberChange = new LineNumberChangeEvent();
            onRemainLinesChange = new RemainLinesChangeEvent();
        }



        public void Init(int width, int height, Rect openRect, Generator generator)
        {
            //Debug.Log($"[Field] Init: width = {width}, height = {height}");

            this.width = width;
            this.height = height;

            Vector2 brickSize = blockPrefab.GetComponent<BoxCollider2D>().size;

            //Debug.Log($"[Field] Init: brickSize = {brickSize}");

            float cellW = openRect.width / width;
            float cellH = openRect.height / height;

            cellSize = Mathf.Min(cellW, cellH);
            fieldScale = cellSize / brickSize.x;

            //Debug.Log($"[Field] Init: cellSize = {cellSize}");

            Vector2 fieldSize = new Vector2(cellSize * width, cellSize * height);

            cellSize = brickSize.x;

            float scaledBrickSize = brickSize.x * fieldScale;

            float hWidth = (openRect.width - fieldSize.x) / 2;
            float hHeight = (openRect.height - fieldSize.y) / 2;

            borders = openRect;
            borders.width = fieldSize.x;
            borders.height = fieldSize.y;
            borders.x += hWidth;
            //borders.y += hHeight;

            ScreenManager.Instance.gameInterface.SetFrame(borders);

            Vector2 zeroPoint = new Vector2(borders.xMin + scaledBrickSize / 2, borders.yMax - scaledBrickSize / 2);

            gameObject.transform.localScale = new Vector3(fieldScale, fieldScale, 1);
            gameObject.transform.position = zeroPoint;
            
            BoxCollider2D topBorderCollider = TopBorder.GetComponent<BoxCollider2D>();
            BoxCollider2D bottomBorderCollider = BottomBorder.GetComponent<BoxCollider2D>();
            BoxCollider2D leftBorderCollider = LeftBorder.GetComponent<BoxCollider2D>();
            BoxCollider2D rightBorderCollider = RightBorder.GetComponent<BoxCollider2D>();

            float size = leftBorderCollider.size.x;

            topBorderCollider.size = new Vector2(borders.width / fieldScale, topBorderCollider.size.y);
            bottomBorderCollider.size = new Vector2(borders.width / fieldScale, bottomBorderCollider.size.y);
            leftBorderCollider.size = new Vector2(leftBorderCollider.size.x, borders.height / fieldScale + size * 2);
            rightBorderCollider.size = new Vector2(rightBorderCollider.size.x, borders.height / fieldScale + size * 2);

            TopBorder.transform.position = new Vector3((borders.xMin + borders.xMax) / 2, borders.yMax, 0f);
            BottomBorder.transform.position = new Vector3((borders.xMin + borders.xMax) / 2, borders.yMin, 0f);
            LeftBorder.transform.position = new Vector3(borders.xMin, (borders.yMin + borders.yMax) / 2, 0f);
            RightBorder.transform.position = new Vector3(borders.xMax, (borders.yMin + borders.yMax) / 2, 0f);

            float ballRadius = ballPrefab.GetComponent<CircleCollider2D>().radius;

            Debug.Log($"[Field] Init: ballRadius = {ballRadius}");

            ballStartY = borders.yMin + ballRadius * fieldScale + .05f;

            Debug.Log($"[Field] Init: ballStartY = {ballStartY}");
            Debug.Log($"[Field] Init: borders.yMin = {borders.yMin}");

            ballStartXMin = borders.xMin + ballRadius;
            ballStartXMax = borders.xMax - ballRadius;

            ballStart.transform.position = new Vector3((ballStartXMax + ballStartXMin) / 2, ballStartY, 0f);
            ballStart.GetComponent<SpriteRenderer>().sprite = gameManager.CurrentSprite;
            ballNextStart.GetComponent<SpriteRenderer>().sprite = gameManager.CurrentSprite;

            this.generator = generator;
        }



        public void StartInfinityGame(int checkpoint = 1)
        {
            Debug.Log("[Field] StartInfinityGame");

            GameManager.gameMode = GameMode.Infinity;

            ClearBlocks();
            ClearBalls();

            isGameOver = false;
            if (throwBallsCoroutine != null)
            {
                StopCoroutine(throwBallsCoroutine);
            }

            if (checkpoint < 1)
                checkpoint = 1;

            lineNumber = checkpoint;
            ballsCount = lineNumber;

            int y = 1;
            brickPointCount = 0;
            SetAimLineVisible(false);
            SetHelpLineVisible(false);
            isBallStartOnPosition = false;
            IsFirstBallPositionSaved = false;
            SetState(FieldState.BeforeStart);
            ballStart.transform.position = new Vector3((ballStartXMax + ballStartXMin) / 2, ballStartY, 0f);
            ballNextStart.SetActive(false);
            ballCountText.transform.position = ballStart.transform.position;
            ballCountText.gameObject.SetActive(true);
            ballCountText.text = $"x{ballsCountForTurn}";
            AddLine(lineNumber, y, false);

            if (GameManager.gameMode == GameMode.Infinity)
            {
                SaveField();
            }
        }



        public void ContinueInfinityGame(XmlNode node)
        {
            Debug.Log("[Field] ContinueInfinityGame");

            GameManager.gameMode = GameMode.Infinity;

            ClearBlocks();
            ClearBalls();

            isGameOver = false;
            if (throwBallsCoroutine != null)
            {
                StopCoroutine(throwBallsCoroutine);
            }

            if (node == null)
            {
                Debug.Log("[Field] ContinueInfinityGame: Node is null");
                StartInfinityGame();
                return;
            }
            else
            {
                if (Load(node))
                {

                    SetAimLineVisible(false);
                    SetHelpLineVisible(false);
                    isBallStartOnPosition = false;
                    IsFirstBallPositionSaved = false;

                    // нужно переделать !!!
                    // Загружать из файла
                    SetState(FieldState.BeforeStart);
                    ballStart.transform.position = new Vector3((ballStartXMax + ballStartXMin) / 2, ballStartY, 0f);
                    //

                    ballNextStart.SetActive(false);
                    ballCountText.transform.position = ballStart.transform.position;
                    ballCountText.gameObject.SetActive(true);
                    ballCountText.text = $"x{ballsCountForTurn}";
                }
                else
                {
                    StartInfinityGame();
                    return;
                }
            }
        }



        public void StartDailyChallenge(int day)
        {
            Debug.Log($"[Field] StartDailyChallenge: {day}");
            
            Random.InitState(day);

            ballsCount = 100;
            lineNumber = Random.Range(56, 73);
            int linesCount = Random.Range(21, 40);

            generator.GenerateRules(width);
            lines = new Queue<List<BlockType>>(generator.GetLines(lineNumber, linesCount, width));

            GameManager.gameMode = GameMode.DailyChallenge;

            ClearBlocks();
            ClearBalls();

            isGameOver = false;
            isWin = false;
            if (throwBallsCoroutine != null)
            {
                StopCoroutine(throwBallsCoroutine);
            }

            brickPointCount = 0;
            SetAimLineVisible(false);
            SetHelpLineVisible(false);
            isBallStartOnPosition = false;
            IsFirstBallPositionSaved = false;
            SetState(FieldState.BeforeStart);
            ballStart.transform.position = new Vector3((ballStartXMax + ballStartXMin) / 2, ballStartY, 0f);
            ballNextStart.SetActive(false);
            ballCountText.transform.position = ballStart.transform.position;
            ballCountText.gameObject.SetActive(true);
            ballCountText.text = $"x{ballsCountForTurn}";
            Score = 0;

            for (int i = 6; i >= 1; i--)
            {
                AddLine(lineNumber, i, false);
                lineNumber++;
            }
            lineNumber--;
        }
        
        

        void NextTurn()
        {
            ballStart.transform.position = nextStart;

            isFirstBallPositionSaved = false;

            ClearBalls();

            SetState(FieldState.PlayerTurn);
        }



        void CreateNewLine()
        {
            lineNumber++;
            int y = 0;

            newBlocks.Clear();

            AddLine(lineNumber, y, true);

            newBlocksCount = 0;
        }



        void ClearBlocks()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                Destroy(blocks[i].gameObject);
            }
            blocks.Clear();
        }



        void ClearBalls()
        {
            for (int i = 0; i < balls.Count; i++)
            {
                ballPool.Despawn(balls[i]);
            }
            balls.Clear();
        }



        void AddLine(int lineNumber, int yPos, bool withAnimation)
        {
            List<BlockType> line = null;
            if (GameManager.gameMode == GameMode.Infinity)
            {
                if (onLineNumberChange != null)
                {
                    onLineNumberChange.Invoke(lineNumber);
                }

                line = generator.GetLine(lineNumber, width);

                if (lineNumber > gameManager.gameInfo.Best)
                {
                    gameManager.gameInfo.Best = lineNumber;
                }
            }
            else
            {
                if (lines.Count > 0)
                {
                    line = lines.Dequeue();
                    if (onRemainLinesChange != null)
                    {
                        onRemainLinesChange.Invoke(lines.Count);
                    }
                }
            }

            if (line != null)
            {
                for (int x = 0; x < line.Count; x++)
                {
                    Block block = null;

                    switch (line[x])
                    {
                        case BlockType.Brick:
                            block = CreateBrick(new Vector2Int(x, yPos), lineNumber);
                            break;

                        case BlockType.DoubleBrick:
                            block = CreateBrick(new Vector2Int(x, yPos), lineNumber * 2, true);
                            break;

                        case BlockType.BallBonus:
                            block = CreateBallBonus(new Vector2Int(x, yPos));
                            break;

                        case BlockType.StarBonus:
                            block = CreateStarBonus(new Vector2Int(x, yPos));
                            break;

                        case BlockType.ShieldBonus:
                            block = CreateShieldBonus(new Vector2Int(x, yPos));
                            break;

                        case BlockType.DoubleBallBonus:
                            block = CreateDoubleBallBonus(new Vector2Int(x, yPos));
                            break;

                        case BlockType.Lazer:
                            int r = Random.Range(0, 99);
                            if (r >= 0 && r < 40)
                                block = CreateHorizontalLazer(new Vector2Int(x, yPos));
                            if (r >= 40 && r < 80)
                                block = CreateVerticalLazer(new Vector2Int(x, yPos));
                            if (r >= 80 && r < 99)
                                block = CreateDoubleLazer(new Vector2Int(x, yPos));
                            break;
                    }

                    if (block != null && withAnimation == true)
                    {
                        block.OnCreate();
                        newBlocks.Add(block);
                    }
                }
            }
        }



        public void RemoveBlock(Block block)
        {
            block.OnBreak();
            Destroy(block.gameObject);
            blocks.Remove(block);
        }



        public void RemoveAllBlocks()
        {
            for (int i = 0; i < blocks.Count;)
            {
                if (blocks[i].BlockType == BlockType.Brick || blocks[i].BlockType == BlockType.DoubleBrick)
                {
                    RemoveBlock(blocks[i]);
                }
                else
                {
                    Destroy(blocks[i].gameObject);
                    blocks.Remove(blocks[i]);
                }
            }
        }



        public Rect GetBorders()
        {
            return borders;
        }



        Brick CreateBrick(Vector2Int position, int life, bool isDouble = false)
        {
            Brick brick = Instantiate(brickPrefab, blockParent, false);
            brick.Init(this, life, isDouble);
            brick.transform.localPosition = new Vector3(position.x * cellSize, -position.y * cellSize, 0f);
            brick.FieldPosition = position;
            blocks.Add(brick);
            isAllBricksBroken = false;

            return brick;
        }



        BallBonus CreateBallBonus(Vector2Int position)
        {
            BallBonus ballBonus = Instantiate(ballBonusPrefab, blockParent, false);
            ballBonus.Init(this);
            ballBonus.transform.localPosition = new Vector3(position.x * cellSize, -position.y * cellSize, 0f);
            ballBonus.FieldPosition = position;

            blocks.Add(ballBonus);

            return ballBonus;
        }



        StarBonus CreateStarBonus(Vector2Int position)
        {
            StarBonus starBonus = Instantiate(starBonusPrefab, blockParent, false);
            starBonus.Init(this);
            starBonus.transform.localPosition = new Vector3(position.x * cellSize, -position.y * cellSize, 0f);
            starBonus.FieldPosition = position;
            blocks.Add(starBonus);

            return starBonus;
        }



        ShieldBonus CreateShieldBonus(Vector2Int position)
        {
            ShieldBonus shieldBonus = Instantiate(shieldBonusPrefab, blockParent, false);
            shieldBonus.Init(this);
            shieldBonus.transform.localPosition = new Vector3(position.x * cellSize, -position.y * cellSize, 0f);
            shieldBonus.FieldPosition = position;
            blocks.Add(shieldBonus);

            return shieldBonus;
        }



        DoubleBallBonus CreateDoubleBallBonus(Vector2Int position)
        {
            DoubleBallBonus doubleBallBonus = Instantiate(doubleBallBonusPrefab, blockParent, false);
            doubleBallBonus.Init(this);
            doubleBallBonus.transform.localPosition = new Vector3(position.x * cellSize, -position.y * cellSize, 0f);
            doubleBallBonus.FieldPosition = position;
            blocks.Add(doubleBallBonus);

            return doubleBallBonus;
        }



        HorizontalLazer CreateHorizontalLazer(Vector2Int position)
        {
            HorizontalLazer horizontalLazer = Instantiate(horizontalLazerPrefab, blockParent, false);
            horizontalLazer.transform.localPosition = new Vector3(position.x * cellSize, -position.y * cellSize, 0f);
            horizontalLazer.FieldPosition = position;
            horizontalLazer.Init(this);
            blocks.Add(horizontalLazer);

            return horizontalLazer;
        }



        VerticalLazer CreateVerticalLazer(Vector2Int position)
        {
            VerticalLazer verticalLazer = Instantiate(verticalLazerPrefab, blockParent, false);
            verticalLazer.transform.localPosition = new Vector3(position.x * cellSize, -position.y * cellSize, 0f);
            verticalLazer.FieldPosition = position;
            verticalLazer.Init(this);
            blocks.Add(verticalLazer);

            return verticalLazer;
        }



        DoubleLazer CreateDoubleLazer(Vector2Int position)
        {
            DoubleLazer doubleLazer = Instantiate(doubleLazerPrefab, blockParent, false);
            doubleLazer.transform.localPosition = new Vector3(position.x * cellSize, -position.y * cellSize, 0f);
            doubleLazer.FieldPosition = position;
            doubleLazer.Init(this);
            blocks.Add(doubleLazer);

            return doubleLazer;
        }



        public Ball CreateBall(Vector2 position, Vector2 angle, float speed)
        {
            Ball ball = ballPool.Spawn(position, Quaternion.identity, ballParent);
            ball.Init(this, gameManager.CurrentSprite);
            balls.Add(ball);
            ball.MoveForward(angle, speed);

            return ball;
        }



        public void ThrowBalls(Vector2 angle)
        {
            throwBallsCoroutine = StartCoroutine(CreateBalls(angle, 120f));
            IsTurnEnded = false;
            SetState(FieldState.Idle);
            ScreenManager.Instance.gameInterface.ShowAbortTurnButton(true);
            ScreenManager.Instance.gameInterface.SetBoostersEnable(false);
            isTimerStarted = true;
            fastForwardTimer = fastForwardDelay;
        }



        public void AddStars(int value)
        {
            gameManager.gameInfo.Stars += value;
        }



        public void SetAimLine(Vector2 start, Vector2 end)
        {
            aimLine.SetPosition(0, start);
            aimLine.SetPosition(1, end);
        }



        public void SetAimLineVisible(bool isVisible)
        {
            aimLine.gameObject.SetActive(isVisible);
        }



        public void SetHelpLine(Vector2 start, Vector2 end)
        {
            helpLine.SetPosition(0, start);
            helpLine.SetPosition(1, end);
        }



        public void SetHelpLineVisible(bool isVisible)
        {
            helpLine.gameObject.SetActive(isVisible);
        }



        public void SetNextStart(Vector2 position)
        {
            nextStart = position;
        }



        public Vector2 GetNextStart()
        {
            return nextStart;
        }



        public void AbortTurn()
        {
            if (fieldState != FieldState.Idle || isAlreadyAberted)
                return;

            if (throwBallsCoroutine != null)
            {
                StopCoroutine(throwBallsCoroutine);
            }

            IsTurnEnded = true;
            isAlreadyAberted = true;

            if (isFirstBallPositionSaved == false)
            {
                SaveBallPosition(balls[0]);
            }

            for (int i = 0; i < balls.Count; i++)
            {
                Ball ball = balls[i];
                ball.StopPhisicSimulation();
                Vector2 newPosition = ball.GetPosition();
                newPosition.y = ballStart.transform.position.y;
                ball.MoveTo(newPosition, 0.2f, ball.GoToNextStartPosition);
            }
        }



        public void SaveBallPosition(Ball ball)
        {
            if (isFirstBallPositionSaved == false)
            {
                isFirstBallPositionSaved = true;

                Vector2 position = ball.GetPosition();
                position.y = ballStart.transform.position.y;

                nextStart = position;

                ballNextStart.transform.position = nextStart;
            }
        }



        public void ShowBallNextStart()
        {
            ballNextStart.SetActive(true);
        }



        public void OnBallArrive(Ball ball)
        {
            if (fieldState == FieldState.Idle)
            {
                ballStopCount++;
                ballPool.Despawn(ball);
                balls.Remove(ball);
            }
        }



        public void OnBlockCreateAnimationEnd()
        {
            newBlocksCount++;
        }


        
        public void SaveField()
        {
            Debug.Log("[Field] SaveField");

            gameManager.gameInfo.canContinue = !isGameOver;
            gameManager.SaveGameInfo();

            XmlDocument xml = new XmlDocument();

            string filePath = Path.Combine(gameManager.Path, gameManager.FileNameField);

            XmlElement root = xml.CreateElement("field");

            xml.AppendChild(root);

            if (!isGameOver)
            {
                root.SetAttribute("lineNumber", lineNumber.ToString());
                root.SetAttribute("ballsCount", ballsCount.ToString());

                for (int i = 0; i < blocks.Count; i++)
                {
                    XmlNode blockNode = blocks[i].Serialize(xml);

                    if (blockNode != null)
                    {
                        root.AppendChild(blockNode);
                    }
                }
            }

            xml.Save(filePath);
        }



        private bool Load(XmlNode node)
        {
            Debug.Log("[Field] Load");
            try
            {
                lineNumber = int.Parse(node.Attributes["lineNumber"].Value);
                ballsCount = int.Parse(node.Attributes["ballsCount"].Value);

                XmlNodeList blockNodes = node.SelectNodes("block");

                brickPointCount = 0;

                foreach (XmlNode item in blockNodes)
                {
                    Block block = null;

                    int x = int.Parse(item.Attributes["x"].Value);
                    int y = int.Parse(item.Attributes["y"].Value);

                    string blockType = item.Attributes["type"].Value;

                    int life = 0;

                    switch (blockType)
                    {
                        case "Brick":
                            life = int.Parse(item.Attributes["life"].Value);
                            block = CreateBrick(new Vector2Int(x, y), life);
                            break;

                        case "DoubleBrick":
                            life = int.Parse(item.Attributes["life"].Value);
                            block = CreateBrick(new Vector2Int(x, y), life, true);
                            break;

                        case "BallBonus":
                            block = CreateBallBonus(new Vector2Int(x, y));
                            break;

                        case "StarBonus":
                            block = CreateStarBonus(new Vector2Int(x, y));
                            break;

                        case "ShieldBonus":
                            block = CreateShieldBonus(new Vector2Int(x, y));
                            break;

                        case "DoubleBallBonus":
                            block = CreateDoubleBallBonus(new Vector2Int(x, y));
                            break;

                        case "HorizontalLazer":
                            block = CreateHorizontalLazer(new Vector2Int(x, y));
                            break;

                        case "VerticalLazer":
                            block = CreateVerticalLazer(new Vector2Int(x, y));
                            break;

                        case "DoubleLazer":
                            block = CreateDoubleLazer(new Vector2Int(x, y));
                            break;
                    }
                }
            }
            catch
            {
                Debug.Log("[Field] Load is failed");
                return false;
            }

            if (onLineNumberChange != null)
            {
                onLineNumberChange.Invoke(lineNumber);
            }

            Debug.Log("[Field] Load is successful");
            return true;
        }



        public void HitHorizontal(int y)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].FieldPosition.y == y)
                {
                    blocks[i].Hit();
                }
            }
        }



        public void HitVertical(int x)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].FieldPosition.x == x)
                {
                    blocks[i].Hit();
                }
            }
        }



        public void OnFastForwardClick()
        {
            if (fieldState == FieldState.Idle)
            {
                Time.timeScale = 2f;
            }
        }



        public void SetGameOver(bool value)
        {
            isGameOver = value;
        }



        public void ContinueAfterGameover()
        {
            SetGameOver(false);

            RemoveLine(height - 1);
            RemoveLine(height - 2);

            SetState(FieldState.PlayerTurn);

            if (GameManager.gameMode == GameMode.Infinity)
            {
                SaveField();
            }
        }



        private void RemoveLine(int index)
        {
            for (int i = 0; i < blocks.Count;)
            {
                if (blocks[i].BlockType == BlockType.Brick && blocks[i].FieldPosition.y == index)
                {
                    RemoveBlock(blocks[i]);
                }
                else
                    i++;
            }
        }



        public void BoosterLastLineExecute()
        {
            int maxY = int.MinValue;
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].BlockType == BlockType.Brick && blocks[i].FieldPosition.y > maxY)
                {
                    maxY = blocks[i].FieldPosition.y;
                }
            }

            RemoveLine(maxY);

            if (GameManager.gameMode == GameMode.Infinity)
            {
                SaveField();
            }
            else
            {
                gameManager.SaveGameInfo();
            }
        }



        public void BoosterDamageExecute()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].BlockType == BlockType.Brick)
                {
                    blocks[i].DoAction("damage");
                }
            }

            if (GameManager.gameMode == GameMode.Infinity)
            {
                SaveField();
            }
            else
            {
                gameManager.SaveGameInfo();
            }
        }



        public void SetState(FieldState state)
        {
            switch (state)
            {
                case FieldState.BeforeStart:
                    ballsCountForTurn = ballsCount;
                    ScreenManager.Instance.gameInterface.SetBoostersEnable(true);
                    ScreenManager.Instance.gameInterface.IsTouchAndDragActive = true;
                    break;

                case FieldState.PlayerTurn:
                    isAlreadyAberted = false;
                    isFastForwardButtonShow = false;
                    isTimerStarted = false;
                    ballsCountForTurn = ballsCount;
                    ballStopCount = 0;
                    ScreenManager.Instance.gameInterface.SetBoostersEnable(true);
                    break;

                case FieldState.Idle:
                    isBallStartOnPosition = false;
                    break;

                case FieldState.BallStartMove:
                    ScreenManager.Instance.gameInterface.ShowAbortTurnButton(false);
                    break;

                case FieldState.BlocksCreate:
                    break;

                case FieldState.BlocksMove:
                    break;

                case FieldState.GameOver:
                    break;
            }

            if (state != FieldState.BeforeStart)
            {
                ScreenManager.Instance.gameInterface.IsTouchAndDragActive = false;
            }

            fieldState = state;
        }



        public void SetPause(bool isPause)
        {
            if (isPause)
            {
                Debug.Log($"pause = {isPause}, store {fieldState}");
                previousState = fieldState;
                SetState(FieldState.Pause);
            }
            else
            {
                Debug.Log($"pause = {isPause}, now {fieldState}, set {previousState}");
                SetState(previousState);
            }

            foreach (Ball ball in balls)
            {
                ball.SetPhisicSimulation(!isPause);
            }
        }



        public void WinGame()
        {
            isWin = true;
        }

        #endregion

        #region COROUTINES

        private IEnumerator CreateBalls(Vector2 angle, float speed)
        {
            for (int i = 0; i < ballsCountForTurn;)
            {
                if (Field.FieldState != FieldState.Pause)
                {
                    CreateBall(BallPosition, angle, speed);

                    ballCountText.text = $"x{ballsCountForTurn - (i + 1)}";
                    i++;

                    //ballText.text = $"{i} / {balls.Count}";

                    yield return new WaitForSeconds(0.08f);
                }
                else
                {
                    yield return new WaitWhile(() => { return Field.FieldState == FieldState.Pause; });
                }
            }

            ballCountText.gameObject.SetActive(false);
            IsTurnEnded = true;
        }



        IEnumerator MoveBallStart(float time)
        {
            float t = 0f;
            Vector2 pos = ballStart.transform.position;

            while (((Vector2)ballStart.transform.position - nextStart).sqrMagnitude > Vector2.kEpsilon)
            {
                ballStart.transform.position = Vector2.Lerp(pos, nextStart, t);
                ballCountText.transform.position = ballStart.transform.position;

                t += Time.deltaTime / time;

                yield return null;
            }

            isBallStartOnPosition = true;
            ballCountText.text = $"x{ballsCount}";
            ballCountText.gameObject.SetActive(true);
        }

        #endregion

        #region NESTED TYPES

        public class ScoreChangeEvent : UnityEvent<int> { }

        public class LineNumberChangeEvent : UnityEvent<int> { }

        public class RemainLinesChangeEvent : UnityEvent<int> { }

        #endregion
    }
}
