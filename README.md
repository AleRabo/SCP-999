# SCP999
Adds the custom role SCP-999

   ![img](https://img.shields.io/github/downloads/AleRabo/Better-RP/total.svg)


   REQUIRES MAPEDITOR REBORN


# INSTALLATION

Download the dll and put it in plugins folder
Download **[Map Editor Reborn]([https://github.com/Michal78900/MapEditorReborn])** dll and put it in plugins folder
Download the json and put it in the schematics folder of MER

#Config

SCP999:
# Whether or not is the plugin enabled?
  is_enabled: true
  # Whether or not is the plugin is in debug mode?
  debug: false
  # Is SCP-999 immortal
  scp999_god_mode: false
  # Configs for the SCP-999 role players turn into.
  scp999_role_config:
    id: 999
    visible_role: Tutorial
    max_health: 999
    name: 'SCP-999'
    description: 'The tickle monster.'
    custom_info: 'SCP-999'
    scale:
      x: 0.5
      y: 0.5
      z: 0.5
    spawn_properties:
      limit: 1
      dynamic_spawn_points:
      - location: Inside330
        chance: 100
      static_spawn_points: []
      role_spawn_points: []
    inventory:
    - 'SCP207'
    - 'Adrenaline'
    - 'Medkit'
    custom_abilities: []
    ammo: {}
    keep_position_on_spawn: false
    keep_inventory_on_spawn: false
    removal_kills_player: true
    keep_role_on_death: false
    spawn_chance: 0
    ignore_spawn_system: false
    keep_role_on_changing_role: false
    broadcast:
    # The broadcast content
      content: ''
      # The broadcast duration
      duration: 10
      # The broadcast type
      type: Normal
      # Indicates whether the broadcast should be shown or not
      show: true
    display_custom_item_messages: true
    custom_role_f_f_multiplier: {}
    console_message: 'You have spawned as a custom role!'
    ability_usage: 'Enter ".special" in the console to use your ability. If you have multiple abilities, you can use this command to cycle through them, or specify the one to use with ".special ROLENAME AbilityNum"'
  # SCP-999 AOE abiliities range.
  abilities:
    Invigorate:
      range: 10
      effect_duration: 5
    Heal:
      range: 20
      effect_duration: 50
    SpeedBoost:
      range: 15
      effect_duration: 4
  # Does the SpeedBoost ability slow SCPs?
  speed_slows_s_c_ps: true
