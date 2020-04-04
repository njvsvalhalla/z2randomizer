
# Zelda 2 Randomizer - Console and Web App

[Use it!](https://zeldaiirandomizer.com)

I based this off of Z2Randomizer by digshake https://bitbucket.org/digshake/z2randomizer/

I converted it to a console app and a web app. The console/web can generate a rom based on a json file as well as preset.

## What did I change?

Other than adding a web interface, I cleaned up the code quite a bit - added interfaces, organized files, extracted things that made sense to be constants/static etc. I did that with the hopes/intention to make things be able to be easily edited (sprites, community hints, etc).

I think I made good progress, and I'm happy with it now, I would still like to do more stuff in Hyrule.cs because I have a thing about 1000+ lines in files.

## Basic asp core app/jqeury/bootsrap - for real?

I'm honestly more of a back end dev. ~~I have minimal experience with react and angular and I think it's a bit overkill to write it in that.~~ I think down the line I will try to  re-do the web site, probably in react because I think I can do a lot better.

## Notes

I refactored things a bit to make easy modifications in places, so a lot of it should be pretty straightforward.

### Sprites

I included a sprite generator linq script to generate the cs file. Place it in the RandomizerCore\Sprites folder.
UpdateSprites() in Hyrule.cs has a switch in it - I'd really like for the switch to use the enum instead of string literals, but ya. Follow the pattern with the rest.
To make it appear in the web randomizer, it needs to be added to the CharacterSprite enum in RandomizerApp\Enums.cs

I left the beam property in there for historical purposes - it wasn't used in the original rando, not sure why it was unused, but if we need it back down the line I kept the property.

This generator should work for patched roms, but if there are any issues, it might need to be modified.

### Community Hints

All community hints are in RandomizerCore\Constants\Text.cs


## Todo

* Output log (console app should be easy, have to think of how to handle web output logs)
* Re-do unit tests
* Re-do the console app
* Perhaps port over the form app, since I am beginning to add stuff?
  
## Wishlist
* Spoiler log??
