@tool
extends EditorPlugin
# # # # # # # # # # # # # # # # # # # # # # # # # # # # # #
#	Hide Folders
#
#	https://github.com/CodeNameTwister/HideFolders
#	author:	"Twister"
# # # # # # # # # # # # # # # # # # # # # # # # # # # # # #
const HIDE_ICON : Texture = preload("res://addons/Hidefolders/images/GuiVisibilityXray.svg")
const DOT_USER : String = "user://editor/hiddenfolders.dat"

var _buffer : Dictionary = {}
var _flg_totals : int = 0
var _tree : Tree = null
var _busy : bool = false

var _menu_service : EditorContextMenuPlugin = null

var _show_hidded_items : bool = true
var _buttons : Array[Button] = []

func _setup() -> void:
	var dir : String = DOT_USER.get_base_dir()
	if !DirAccess.dir_exists_absolute(dir):
		DirAccess.make_dir_recursive_absolute(dir)
		return
	if FileAccess.file_exists(DOT_USER):
		var cfg : ConfigFile = ConfigFile.new()
		if OK != cfg.load(DOT_USER):return
		_buffer = cfg.get_value("DAT", "PTH", {})
		_show_hidded_items = bool(cfg.get_value("ShowHideItems", "Enabled", true))

#region callbacks
func _moved_callback(a : String, b : String ) -> void:
	if a != b:
		if _buffer.has(a):
			_buffer[b] = _buffer[a]
			_buffer.erase(a)

func _remove_callback(path : String) -> void:
	if _buffer.has(path):
		_buffer.erase(path)
#endregion

func _on_hidde_cmd(paths : PackedStringArray) -> void:
	for path : String in paths:
		if FileAccess.file_exists(path):continue
		if DirAccess.dir_exists_absolute(path):
			if _buffer.has(path):
				_buffer[path] = true
			else:
				_buffer[path] = false
	if _buffer.has("res://"):
		_buffer.erase("res://")
	if _buffer.size() > 0:
		_def_update()

func _def_update() -> void:
	update.call_deferred()

func update() -> void:
	if _buffer.size() == 0:return
	if _busy:return
	_busy = true
	var root : TreeItem = _tree.get_root()
	var item : TreeItem = root.get_first_child()

	while null != item and item.get_metadata(0) != "res://":
		item = item.get_next()
	_flg_totals = 0

	_explore(item)
	set_deferred(&"_busy", false)

func _explore(item : TreeItem) -> void:
	var meta : Variant = item.get_metadata(0)
	if _buffer.has(meta):
		var v_flag : bool = _buffer[meta]
		for i : TreeItem in item.get_children():
			i.visible = v_flag

		if v_flag == false:
			_flg_totals += 1
			item.collapsed = true
			var c : Color = item.get_icon_modulate(0)
			c.a = 0.5
			item.set_icon_overlay(0, HIDE_ICON)
			item.set_custom_color(0, Color.DARK_GRAY)
			item.set_icon_modulate(0,c)
			
			item.visible = _show_hidded_items and item.get_parent().visible
			
			#TODO: Ignore totals and return now
			if _flg_totals >= _buffer.size():
				return
		else:
			var c : Color = item.get_icon_modulate(0)
			c.a = 1.0
			item.set_custom_color(0, Color.WHITE)
			item.set_icon_modulate(0, c)
			item.set_icon_overlay(0, null)
			_buffer.erase(meta)
			return

	for i : TreeItem in item.get_children():
		_explore(i)

func get_buffer() -> Dictionary: return _buffer

func _on_collapsed(item : TreeItem) -> void:
	var v : Variant = item.get_metadata(0)
	if _buffer.has(v):
		for _i : TreeItem in item.get_children():
			_i.visible = false
		item.collapsed = true
		var c : Color = item.get_icon_modulate(0)
		c.a = 0.5
		item.set_icon_overlay(0, HIDE_ICON)
		item.set_custom_color(0, Color.DARK_GRAY)
		item.set_icon_modulate(0,c)

# credits: collapse_folders
func find_button_container(node: Node) -> Array[Container]:
	var containers: Array[Container] = []
	for child in node.get_children():
		if child is MenuButton and child.tooltip_text == tr("Sort Files"):
			containers.append(node)
		
		for container in find_button_container(child):
			containers.append(container)

	return containers

func _ready() -> void:
	var dock : FileSystemDock = EditorInterface.get_file_system_dock()
	var fs : EditorFileSystem = EditorInterface.get_resource_filesystem()
	_n(dock)

	_tree.item_collapsed.connect(_on_collapsed)

	add_context_menu_plugin(EditorContextMenuPlugin.CONTEXT_SLOT_FILESYSTEM, _menu_service)

	dock.folder_moved.connect(_moved_callback)
	dock.folder_removed.connect(_remove_callback)
	dock.folder_color_changed.connect(_def_update)
	fs.filesystem_changed.connect(_def_update)
	_def_update()
	
	var containers :  Array[Container] = find_button_container(dock)
	for container : Container in containers:
		var button : Button = Button.new()
		button.tooltip_text = "Show/Hide Folders hider"
		button.flat = true
		button.icon = preload("res://addons/Hidefolders/images/GuiVisibilityXray.svg")
		button.toggle_mode = true
		button.toggled.connect(_show_hide_enable)
		button.button_pressed = _show_hidded_items
		container.add_child(button)
		container.move_child(button, container.get_child_count() - 2)
		_buttons.append(button)
		
func _show_hide_enable(toggle : bool) -> void:
	for b : Button in _buttons:
		b.button_pressed = toggle
	_show_hidded_items = !toggle
	_def_update()

func _enter_tree() -> void:
	_setup()

	_menu_service = ResourceLoader.load("res://addons/Hidefolders/menu_item.gd").new()
	_menu_service.ref_plug = self
	_menu_service.hide_folders.connect(_on_hidde_cmd)


func _exit_tree() -> void:
	if is_instance_valid(_menu_service):
		remove_context_menu_plugin(_menu_service)
		_menu_service.ref_plug = null

	var dock : FileSystemDock = EditorInterface.get_file_system_dock()
	var fs : EditorFileSystem = EditorInterface.get_resource_filesystem()
	if dock.folder_moved.is_connected(_moved_callback):
		dock.folder_moved.disconnect(_moved_callback)
	if dock.folder_removed.is_connected(_remove_callback):
		dock.folder_removed.disconnect(_remove_callback)
	if dock.folder_color_changed.is_connected(_def_update):
		dock.folder_color_changed.disconnect(_def_update)
	if fs.filesystem_changed.is_connected(_def_update):
		fs.filesystem_changed.disconnect(_def_update)

	#region user_dat
	var cfg : ConfigFile = ConfigFile.new()
	for k : String in _buffer.keys():
		if !DirAccess.dir_exists_absolute(k):
			_buffer.erase(k)
			continue
	cfg.set_value("DAT", "PTH", _buffer)
	cfg.set_value("ShowHideItems", "Enabled", _show_hidded_items)
	if OK != cfg.save(DOT_USER):
		push_warning("Error on save HideFolders!")
	cfg = null
	#endregion

	_menu_service = null
	_buffer.clear()
	
	for x : Button in _buttons:
		x.queue_free()
	
#region rescue_fav
func _n(n : Node) -> bool:
	if n is Tree:
		var t : TreeItem = (n.get_root())
		if null != t:
			t = t.get_first_child()
			while t != null:
				if t.get_metadata(0) == "res://":
					_tree = n
					return true
				t = t.get_next()
	for x in n.get_children():
		if _n(x): return true
	return false
#endregion
