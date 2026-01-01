# Getting Started with MONOE.exe

Welcome to MONOE.exe! This guide will walk you through creating your first small game using Lua scripting. We'll build a simple "Pong-like" game where a ball bounces around and you control a paddle.

## Prerequisites

- MONOE.exe engine installed
- Basic knowledge of Lua programming
- A text editor

## Step 1: Set Up Your Project

Create a new directory for your game project. Copy the `libraries` folder from the MONOE.exe installation into your project directory.

Create a `project.lua` file in your project root:

```lua
local engine = require('monoelib.engine')

-- Define your game object
monoe.game = {}

function monoe.game.ready()
    print("Game is ready!")
    -- Initialize your game here
end

function monoe.game.process(delta)
    -- Update game logic every frame
    -- delta is the time elapsed since last frame
end

function deps()
    -- Return any custom DLL dependencies (empty for now)
    return {}
end

-- Register the game with the engine
engine.qualify(monoe.game)
```

## Step 2: Create Game Objects

Create a new Lua file called `game.lua` in your project directory:

```lua
local entity = require('monoelib.types.entity')
local sprite = require('monoelib.types.sprite')
local event = require('monoelib.event')

-- Game variables
local ball
local paddle
local ballSpeed = {x = 200, y = 150}
local paddleSpeed = 300

function initGame()
    -- Create the ball
    ball = entity.new()
    local ballSprite = sprite.new("ball.png")  -- You'll need a ball.png image
    ball:attach(ballSprite)
    ball:position(400, 300)  -- Center of screen

    -- Create the paddle
    paddle = entity.new()
    local paddleSprite = sprite.new("paddle.png")  -- You'll need a paddle.png image
    paddle:attach(paddleSprite)
    paddle:position(400, 550)  -- Bottom of screen
end

function updateBall(delta)
    -- Move the ball
    local x, y = ball:position()
    x = x + ballSpeed.x * delta
    y = y + ballSpeed.y * delta

    -- Bounce off walls
    if x <= 0 or x >= 800 then  -- Assuming 800px width
        ballSpeed.x = -ballSpeed.x
    end
    if y <= 0 then
        ballSpeed.y = -ballSpeed.y
    end

    -- Check paddle collision (simple AABB)
    local px, py = paddle:position()
    if y >= py - 20 and y <= py + 20 and x >= px - 50 and x <= px + 50 then
        ballSpeed.y = -ballSpeed.y
    end

    -- Reset if ball goes off bottom
    if y > 600 then  -- Assuming 600px height
        x, y = 400, 300
        ballSpeed.x = 200
        ballSpeed.y = 150
    end

    ball:position(x, y)
end

function updatePaddle(delta)
    -- Move paddle with keyboard input
    local keyboard = require('monoelib.io.keyboard')

    local px, py = paddle:position()

    if keyboard.down("ui_left") then
        px = px - paddleSpeed * delta
    elseif keyboard.down("ui_right") then
        px = px + paddleSpeed * delta
    end

    -- Keep paddle on screen
    if px < 50 then px = 50 end
    if px > 750 then px = 750 end

    paddle:position(px, py)
end

-- Initialize when the game is ready
event.once('ready', function()
    initGame()
end)

-- Update every frame
event.subscribe('process', function(delta)
    updateBall(delta)
    updatePaddle(delta)
end)

return {}
```

## Step 3: Update project.lua to Load Your Game

Modify your `project.lua` to load the game script:

```lua
local engine = require('monoelib.engine')

monoe.game = {}

function monoe.game.ready()
    print("Game is ready!")
    -- Load the main game script
    engine.load('mainGame', 'game')
end

function deps()
    return {}
end

engine.qualify(monoe.game)
```

## Step 4: Add Input Handling

The keyboard input in the example above uses the `monoelib.io.keyboard` module. Make sure you have the necessary IO libraries available.

## Step 5: Run Your Game

1. Place your `project.lua` and `game.lua` in the project directory
2. Add `ball.png` and `paddle.png` images to the directory
3. Run MONOE.exe with your project directory as the working directory

## Step 6: Enhance Your Game

- Add scoring system
- Add sound effects using the audio library
- Add multiple balls or power-ups
- Implement AI for computer-controlled paddles

## Using the Runtime Shell

While your game is running, you can use the built-in shell to:

- Execute Lua code: `print("Hello from shell!")`
- Reload scripts: `:reload`
- Inspect variables: `:dump _G`
- Check performance: `:stats`

## Next Steps

- Explore the [API documentation](./api/lua.md) for more features
- Check out [shell commands](./shell.md) for debugging
- Learn about [hot reloading](./meta-runtime.md) for faster development

Happy game development with MONOE.exe!