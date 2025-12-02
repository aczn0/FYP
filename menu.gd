extends Control


func _on_train_q_pressed() -> void:
	print("trainq")
	get_tree().change_scene_to_file("res://tile_map.tscn")

func _on_play_q_pressed() -> void:
	print("playq")

func _on_train_sarsa_pressed() -> void:
	print("trainsarsa")
	get_tree().change_scene_to_file("res://tile_map2.tscn")


func _on_play_sarsa_pressed() -> void:
	print("playsarsa")
