extends CodeEdit

# ref: https://coolors.co/palette/fbf8cc-fde4cf-ffcfd2-f1c0e8-cfbaf0-a3c4f3-90dbf4-8eecf5-98f5e1-b9fbc0

var LUA_KEYWORDS: Array[String] = [
	'break' , 'do' , 'else' , 'elseif' , 'end',
	'for' , 'function' , 'if' , 'in' , 'local' , 
	'repeat' , 'return' , 'then' , 'true' , 'until' , 'while',
	'goto'
]

var LUA_LIT: Array[String] = [
	'and', 'nil', 'not', 'or', 'false', 
]

var LUA_GLOBAL_FUNCS := [
	'assert', 'collectgarbage', 'dofile', 'error',
	'getmetatable', 'ipairs', 'load', 'loadfile',
	'next', 'pairs', 'pcall', 'print',
	'rawequal', 'rawget', 'rawlen', 'rawset',
	'require', 'select', 'setmetatable',
	'tonumber', 'tostring', 'type', 'xpcall',
	'_G', '_ENV', '_VERSION',
]

var LUA_NAMESPACES := [
	'math', 'string', 'table', 'io',
	'os', 'coroutine', 'debug', 'utf8', 'monoelib'
]

var LUA_SUGGESTIONS: PackedStringArray = (
	LUA_KEYWORDS
	+ LUA_LIT
	+ LUA_GLOBAL_FUNCS
	+ LUA_NAMESPACES
)

func get_keywords() -> Dictionary[String, Color]:
	var keywords: Dictionary[String, Color] = {}

	var keyword_color := Color('FFCFD2')
	var lit_color := Color('F1C0E8')
	var builtin_color := Color('FBF8CC')

	for kw in LUA_KEYWORDS: keywords[kw] = keyword_color
	for lt in LUA_LIT: keywords[lt] = lit_color
	for kw in LUA_GLOBAL_FUNCS: keywords[kw] = builtin_color
	for kw in LUA_NAMESPACES: keywords[kw] = Color('90DBF4')

	return keywords

func init_code_editor() -> void:
	var highlighter: CodeHighlighter = CodeHighlighter.new()
	highlighter.number_color = Color("8EECF5")
	highlighter.symbol_color = Color('CFBAF0')
	highlighter.function_color = Color('FDE4CF')
	highlighter.keyword_colors = get_keywords()
	
	self.syntax_highlighter = highlighter

func _ready() -> void:
	self.auto_brace_completion_enabled = true
	self.add_string_delimiter('[[', ']]', false)
	self.delimiter_comments = ['--']
	self.indent_size = 2
	self.indent_use_spaces = true
	self.indent_automatic_prefixes = [
		'do', 'then', '{', '(', '[',
	]
	self.indent_automatic = true
	self.code_completion_enabled = true

	connect("code_completion_requested", _on_completion_requested)
	init_code_editor()

func _on_completion_requested() -> void:
	var prefix := get_text_for_code_completion().strip_edges()
	if prefix.is_empty():
		return

	for word in LUA_SUGGESTIONS:
		if word.begins_with(prefix):
			add_code_completion_option(
				CodeEdit.KIND_PLAIN_TEXT,
				word,
				word,
				Color.WHITE
			)

	request_code_completion()
