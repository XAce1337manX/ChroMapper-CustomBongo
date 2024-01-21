# ChroMapper-CustomBongo

This plugin for ChroMapper replaces the existing bongo cat with custom images. The plugin uses files in the same folder as the plugin itself:

| File name   | Bongo State      |
|-------------|------------------|
| `none.png`  | Idle             |
| `left.png`  | Left hand swing  |
| `right.png` | Right hand swing |
| `both.png`  | Double swing     |

The plugin will create a `config.json` which allows for tweaking the custom image appearance:

| Key       | Effect            |
|-----------|-------------------|
| `ScaleX`  | Horizontal scale  |
| `ScaleY`  | Vertical scale    |
| `OffsetX` | Horizontal offset |
| `OffsetY` | Vertical offset   |