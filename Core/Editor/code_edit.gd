extends CodeEdit

func _ready():
	var lua := CodeHighlighter.new()
	syntax_highlighter = lua

	var keyword_color   = Color("#FF6F91")  # pink
	var string_color    = Color("#6BCB77")  # pastel green
	var comment_color   = Color("#A0AEC0")  # grayish blue
	var builtin_color   = Color("#4D96FF")  # pastel blue

	# -------------------
	# LUA KEYWORDS
	# -------------------
	var keywords = [
		"and", "or", "not",
		"if", "then", "elseif", "else", "end",
		"for", "while", "repeat", "until",
		"do", "break", "return",
		"local", "in", "function",
	]

	for k in keywords:
		lua.add_keyword_color(k, keyword_color)

	# -------------------
	# BUILTINS
	# -------------------
	var builtins = [
		"print", "pairs", "ipairs", "next",
		"tostring", "tonumber", "type",
		"assert", "error", "pcall", "xpcall",
		"require"
	]

	for b in builtins:
		lua.add_keyword_color(b, builtin_color)

	lua.add_color_region("--", "", comment_color, true)

	lua.add_color_region("\"", "\"", string_color)
	lua.add_color_region("'", "'", string_color)


	minimap_draw = true
