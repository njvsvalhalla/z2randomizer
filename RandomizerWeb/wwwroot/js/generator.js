$().ready()
{
    //credits to zer0-1one : https://github.com/zero0-1one/bitarray
    const BLOCK_BIT_NUM = Uint32Array.BYTES_PER_ELEMENT * 8;
    const BIT_MOVE = Math.log2(BLOCK_BIT_NUM);
    const BIT_MASK = (1 << BIT_MOVE) - 1

      function BitArray(data) {
        var blocksize = (data.length + BLOCK_BIT_NUM - 1) >> BIT_MOVE;

          var arr = new Uint32Array(blocksize);
    
          for (var _i4 = 0; _i4 < blocksize; _i4++) {
            var v = 0;
    
            for (var _j = 0; _j < BLOCK_BIT_NUM; _j++) {
              v += Number(data[_i4 * BLOCK_BIT_NUM + _j] || 0) << _j;
            }
    
            arr[_i4] = v;
          }
    
          return arr;
      }

      function getFromBa(ba,index) {
        let majorIndex = index >> BIT_MOVE
        let minorIndex = index & BIT_MASK
        return (ba[majorIndex] >> minorIndex) & 1
      }

    const flags = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz1234567890!@#$";

    function getIntFromBool(bool) {
        return bool | 0;
    }

    function getFlagId(id) {
        return getIntFromBool(document.getElementsByClassName(`flag${id}`)[0].checked);
    }

    function getItemDropValue(id,size) {
        return getIntFromBool(document.getElementById(`${size}DropPool_${id}__Selected`).checked);
    }

    function getValueFromDropdown(id) {
        return document.getElementById(id).value;
    }

    function flag(ba) {
        return flags[Array.from(ba)[0]];
    }

    function generateFlags() {
        var flagStr = "";
        flagStr += flag(BitArray([getFlagId(1),getFlagId(2),getFlagId(3),getFlagId(4),getFlagId(5),getFlagId(6)]));
        flagStr += flag(BitArray([getFlagId(7),getFlagId(8),getFlagId(9),getFlagId(10),getFlagId(11),getFlagId(12)]));
        flagStr += flag(BitArray([getFlagId(13),getFlagId(14),getFlagId(15),getFlagId(16),getFlagId(17),getFlagId(18)]));
        flagStr += flag(BitArray([getFlagId(19),getFlagId(20),getFlagId(21),getFlagId(22),getFlagId(23),getFlagId(24)]));

        var ba = BitArray([getValueFromDropdown('SettingsModel_StartingHeartContainers')]);
        var baTwo = BitArray([getValueFromDropdown('SettingsModel_StartingTechs')]);
        
        flagStr += flag(BitArray([getFromBa(ba,0),getFromBa(ba,1),getFromBa(ba,2),getFromBa(baTwo,0),getFromBa(baTwo,1),getFromBa(baTwo,2)]));
        flagStr += flag(BitArray([getFlagId(25),getFlagId(26),getFromBa(ba,3),getFlagId(27),getFlagId(28),getFlagId(29)]));

        ba = BitArray([getValueFromDropdown('SettingsModel_AttackEffectiveness')]);
        flagStr += flag(BitArray([getFlagId(30),getFlagId(31),getFromBa(ba,0),getFromBa(ba,1),getFromBa(ba,2),getFlagId(32)]));

        flagStr += flag(BitArray([getFlagId(34),getFlagId(35),getFlagId(36),getFlagId(37),getFlagId(38),getFlagId(39)]));
        flagStr += flag(BitArray([getFlagId(40),getFlagId(41),getFlagId(42),getFlagId(43),getFlagId(44),getFlagId(45)]));

        ba = BitArray([getValueFromDropdown('SettingsModel_MagicEffectiveness')]);
        flagStr += flag(BitArray([getFromBa(ba,0),getFromBa(ba,1),getFromBa(ba,2),getFlagId(46),getFlagId(47),getFlagId(48)]));

        ba = BitArray([getValueFromDropdown('SettingsModel_NumberOfPalacesToComplete')]);
        flagStr += flag(BitArray([getFlagId(49),getFlagId(50),getFromBa(ba,0),getFromBa(ba,1),getFromBa(ba,2),getFlagId(51)]));

        flagStr += flag(BitArray([getFlagId(52),getFlagId(53),getFlagId(54),getFlagId(55),getFlagId(56),getFlagId(57)]));

        ba = BitArray([getValueFromDropdown('SettingsModel_LifeEffectiveness')]);
        flagStr += flag(BitArray([getFromBa(ba,0),getFromBa(ba,1),getFromBa(ba,2),getFlagId(58),getFlagId(59),getFlagId(60)]));

        ba = BitArray([getValueFromDropdown('SettingsModel_MaxHeartContainers')]);
        baTwo = BitArray([getValueFromDropdown('SettingsModel_HiddenPalace')]);
        flagStr += flag(BitArray([getFromBa(ba,0),getFromBa(ba,1),getFromBa(ba,2),getFromBa(ba,3),getFromBa(baTwo,0),getFromBa(baTwo,1)]));

        ba = BitArray([getValueFromDropdown('SettingsModel_HiddenKasuto')]);
        flagStr += flag(BitArray([getFromBa(ba,0),getFromBa(ba,1),getFlagId(61),getFlagId(62),getItemDropValue(0,'Small'),getItemDropValue(1,'Small')]));

        flagStr += flag(BitArray([getItemDropValue(2,'Small'),getItemDropValue(3,'Small'),getItemDropValue(4,'Small'),getItemDropValue(5,'Small'),getItemDropValue(6,'Small'),getItemDropValue(7,'Small')]));
        flagStr += flag(BitArray([getItemDropValue(0,'Large'),getItemDropValue(1,'Large'),getItemDropValue(2,'Large'),getItemDropValue(3,'Large'),getItemDropValue(4,'Large'),getItemDropValue(5,'Large')]));

        ba = BitArray([getValueFromDropdown('SettingsModel_HintType')]);
        flagStr += flag(BitArray([getItemDropValue(6,'Large'),getItemDropValue(7,'Large'),getFromBa(ba,0),getFromBa(ba,1),getFlagId(63),getFlagId(64)]));

        $("#flags").val(flagStr);
    }

    $('.usedForFlags').change(function() {
        generateFlags();
    });

    $('#generateBtn').click(function () {
        var seed = Math.round(Math.random() * (2147438647 - 1000000000) + 1000000000);
        $('#seed').val(seed);
    });

    $('#SettingsModel_ShuffleStartingItems').click(function () {
        shuffleStartingItems();
    })

    $('#SettingsModel_ShuffleStartingSpells').click(function () {
        shuffleStartingSpells();
    })

    $('#SettingsModel_AllowPalacesToSwapContinents').click(function () {
        allowPalacesToSwapContinents();
    });

    $('#SettingsModel_ShuffleEncounters').click(function () {
        shuffleEncounters();
    });
    
    $('#SettingsModel_ShufflePalaceRooms').click(function () {
        shufflePalaceRooms();
    });

    $('#SettingsModel_RemoveThunderbird').click(function () {
        removeThunderbird();            
    });

    $('#SettingsModel_ShuffleAllExperienceNeeded').click(function () {
        shuffleAllExperience();
    });

    $('#SettingsModel_ShuffleOverworldEnemies').click(function () {
        shuffleOverworldEnemies();
    });

    $('#SettingsModel_ShufflePalaceItems').click(function () {
        checkForMixOverworldShuffle();
    });
    
    $('#SettingsModel_ShuffleOverworldItems').click(function () {
        shuffleOverworldItems();
    });

    $('#SettingsModel_ManuallySelectDrops').click(function () {
        manuallySelectDrops();
    });

    function shuffleStartingItems() {
        if (document.getElementById('SettingsModel_ShuffleStartingItems').checked) {
            $(".startingItem").attr("disabled", true);
            $(".startingItem").prop("checked", false);
        }
        else {
            $(".startingItem").attr("disabled", false);
        }
    }


    function shuffleStartingSpells() {
        if (document.getElementById('SettingsModel_ShuffleStartingSpells').checked) {
            $(".startingSpell").attr("disabled", true);
            $(".startingSpell").prop("checked", false);
        }
        else {
            $(".startingSpell").attr("disabled", false);
        }
    }

    function allowPalacesToSwapContinents()  {
        if (document.getElementById('SettingsModel_AllowPalacesToSwapContinents').checked) {
            $(".greatPalaceInShuffle").attr("disabled", false);
        }
        else {
            $(".greatPalaceInShuffle").attr("disabled", true);
        }
    }

    function shuffleEncounters() {
        if (document.getElementById('SettingsModel_ShuffleEncounters').checked) {
            $(".unsafePathEncounters").attr("disabled", false);
        }
        else {
            $(".unsafePathEncounters").attr("disabled", true);
        }
    }

    function checkForMixOverworldShuffle() {
        if (document.getElementById('SettingsModel_ShuffleOverworldItems').checked
            && document.getElementById('SettingsModel_ShufflePalaceItems').checked) {
            $(".mixOverworldItems").attr("disabled", false);
        }
        else {
            $(".mixOverworldItems").attr("disabled", true);
            $(".mixOverworldItems").prop("checked", false);
        }
    }
    
    function shufflePalaceRooms() {
        if (document.getElementById('SettingsModel_ShufflePalaceRooms').checked) {
            $(".palaceShuffleOptions").attr("disabled", false);
            // $(".thunderbirdRequired").prop("checked", false);

        }
        else {
            // $(".thunderbirdRequired").prop("checked", true);
            // $(".removeThunderbird").prop("checked", false);
            // $(".shortenPalace").prop("checked", false);
            $(".palaceShuffleOptions").attr("disabled", true);
        }
    }

    function removeThunderbird() {
        if (document.getElementById('SettingsModel_RemoveThunderbird').checked) {
            $(".thunderbirdRequired").attr("disabled", true);
                $(".thunderbirdRequired").prop("checked", false);
        }
        else {
            $(".thunderbirdRequired").attr("disabled", false);
        }
    }

    function shuffleAllExperience() {
        if (document.getElementById('SettingsModel_ShuffleAllExperienceNeeded').checked) {
            $(".shuffleExperience").attr("disabled", true);
            $(".shuffleExperience").prop("checked", true);
        }
        else {
            $(".shuffleExperience").attr("disabled", false);
            $(".shuffleExperience").prop("checked", false);
        }
    }

    function shuffleOverworldEnemies() {
        if (document.getElementById('SettingsModel_ShuffleOverworldEnemies').checked) {
            $(".mixEnemies").attr("disabled", false);
        }
        else {
            $(".mixEnemies").attr("disabled", true);
            $(".mixEnemies").prop("checked", false);
        }
    }

    function shuffleOverworldItems() {
        checkForMixOverworldShuffle()
        if (document.getElementById('SettingsModel_ShuffleOverworldItems').checked) {
            $(".shufflePbag").attr("disabled", false);
        }
        else {
            $(".shufflePbag").attr("disabled", true);
            $(".shufflePbag").prop("checked", false);
        }
    }

    function manuallySelectDrops() {
        if (document.getElementById('SettingsModel_ManuallySelectDrops').checked) {
            $(".itemDrops").attr("disabled", false);
        }
        else {
            $(".itemDrops").attr("disabled", true);
        }
    }

    shuffleStartingItems();
    shuffleStartingSpells();
    allowPalacesToSwapContinents();
    shuffleEncounters();
    checkForMixOverworldShuffle();
    shufflePalaceRooms();
    removeThunderbird();
    shuffleAllExperience();
    shuffleOverworldEnemies();
    shuffleOverworldItems();
    manuallySelectDrops();
    generateFlags();
}