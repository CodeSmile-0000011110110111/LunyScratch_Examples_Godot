extends RigidBody3D

@export var paddle_hit_sound: AudioStream
@onready var audio_player: AudioStreamPlayer3D = $AudioStreamPlayer3D

func _ready():
	body_entered.connect(_on_body_entered)
	
	if audio_player == null:
		audio_player = AudioStreamPlayer3D.new()
		add_child(audio_player)

func _on_body_entered(body: Node3D):
	if body.is_in_group("Paddle"):
		if audio_player and paddle_hit_sound:
			audio_player.stream = paddle_hit_sound
			audio_player.play()
