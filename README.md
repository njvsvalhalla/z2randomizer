
# Zelda 2 Randomizer - Console and Web App

[Use it!](https://zeldaiirandomizer.com)

I based this off of Z2Randomizer by digshake https://bitbucket.org/digshake/z2randomizer/

I converted it to a console app and a web app. The console/web can generate a rom based on a json file as well as preset.

Other than that, I didn't really change TOO much. I did clean up a good chunk of the code though trying to make it more readable.

## Basic asp core app/jqeury/bootsrap - for real?

I'm honestly more of a back end dev. I have minimal experience with react and angular and I think it's a bit overkill to write it in that.

That being said, I'd love to have someone help with styling.  

## What did I change?

I cleaned up some variable names, formatting, various commented out code (it's in source in case it's needed again), extracted the rom generation to a method.

## Todo

* Output log (console app should be easy, have to think of how to handle web output logs)
* Make colors work..
* Various CI/"prod" stuff (have small things to finish, ie run unit tests on build, deploy to a staging area/prod - but this is moot since I'll probably move it to the cloud)
* Unit test validation method
* Add the other two presets
* LOOOOOOOOOGS
* I'd like to go into the core and move things into seperate classes (ie enums)
* Tooltips
  
## Wishlist
* Spoiler log??
