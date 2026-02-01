-- monoe.exe engine's settings
-- auto-gen glue, based on monoe.exe@3.0.9

project = {
  icon = 'icon.png', -- Path to the project icon,
  dev_name = 'unknown', -- Main developer name,
  company_name = 'unknown', -- The company's name,
  debug = false, -- Enable debug features,
}

window = {
  size = { 1200, 720 }, -- Initial window size, X ; Y,
  resizable = true, -- Allow window resizing,
  title = 'APP2FOU', -- Window's title,
  transparent = false, -- If true, the Window's background can be transparent. This is best used with embedded windows. Note: Transparency support is implemented on Linux, macOS and Windows, but availability might vary depending on GPU driver, display manager, and compositor capabilities.,
  max_size = { 0, 0 }, -- If non-zero, the Window can't be resized to be bigger than this size.,
  min_size = { 0, 0 }, -- If non-zero, the Window can't be resized to be smaller than this size.,
  exclusive = false, -- If true, the Window will be in exclusive mode. Exclusive windows are always on top of their parent and will block all input going to the parent Window.,
  extend_to_title = false, -- If true, the Window contents is expanded to the full size of the window, window title bar is transparent.,
}

engine = {
  max_fps = 0, -- If non-zero, limits the game's frame rate,
  time_scale = 1, -- The speed multiplier at which the in-game clock updates, compared to real time. For example, if set to 2.0 the game runs twice as fast, and if set to 0.5 the game runs half as fast.,
}