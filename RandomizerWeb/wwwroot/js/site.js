$().ready();
{
    //credits to zer0-1one : https://github.com/zero0-1one/bitarray
    const BLOCK_BIT_NUM = Uint32Array.BYTES_PER_ELEMENT * 8;
    const BIT_MOVE = Math.log2(BLOCK_BIT_NUM);
    const BIT_MASK = (1 << BIT_MOVE) - 1;

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

    function getFromBa(ba, index) {
        let majorIndex = index >> BIT_MOVE;
        let minorIndex = index & BIT_MASK;
        return (ba[majorIndex] >> minorIndex) & 1;
    }

    const flags =
        "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz1234567890!@#$";

    function getIntFromBool(bool) {
        return bool | 0;
    }

    function getFlagId(id) {
        return getIntFromBool(
            document.getElementsByClassName(`flag${id}`)[0].checked
        );
    }

    function getItemDropValue(id, size) {
        return getIntFromBool(
            document.getElementById(`${size}DropPool_${id}__Selected`).checked
        );
    }

    function setItemDropVal(id, size, val) {
        $(`#${size}DropPool_${id}__Selected`).prop("checked", getRealVal(val));
    }

    function getValueFromDropdown(id) {
        return document.getElementById(id).value;
    }

    function flag(ba) {
        return flags[Array.from(ba)[0]];
    }

    var dontGenerate = false;

    function generateFlags() {
        if (dontGenerate)
            return;

        var flagStr = "";
        flagStr += flag(
            BitArray([
                getFlagId(1),
                getFlagId(2),
                getFlagId(3),
                getFlagId(4),
                getFlagId(5),
                getFlagId(6),
            ])
        );
        flagStr += flag(
            BitArray([
                getFlagId(7),
                getFlagId(8),
                getFlagId(9),
                getFlagId(10),
                getFlagId(11),
                getFlagId(12),
            ])
        );
        flagStr += flag(
            BitArray([
                getFlagId(13),
                getFlagId(14),
                getFlagId(15),
                getFlagId(16),
                getFlagId(17),
                getFlagId(18),
            ])
        );
        flagStr += flag(
            BitArray([
                getFlagId(19),
                getFlagId(20),
                getFlagId(21),
                getFlagId(22),
                getFlagId(23),
                getFlagId(24),
            ])
        );

        var ba = BitArray([
            getValueFromDropdown("SettingsModel_StartingHeartContainers"),
        ]);
        var baTwo = BitArray([getValueFromDropdown("SettingsModel_StartingTechs")]);

        flagStr += flag(
            BitArray([
                getFromBa(ba, 0),
                getFromBa(ba, 1),
                getFromBa(ba, 2),
                getFromBa(baTwo, 0),
                getFromBa(baTwo, 1),
                getFromBa(baTwo, 2),
            ])
        );
        flagStr += flag(
            BitArray([
                getFlagId(25),
                getFlagId(26),
                getFromBa(ba, 3),
                getFlagId(27),
                getFlagId(28),
                getFlagId(29),
            ])
        );

        ba = BitArray([getValueFromDropdown("SettingsModel_AttackEffectiveness")]);
        flagStr += flag(
            BitArray([
                getFlagId(30),
                getFlagId(31),
                getFromBa(ba, 0),
                getFromBa(ba, 1),
                getFromBa(ba, 2),
                getFlagId(32),
            ])
        );

        flagStr += flag(
            BitArray([
                getFlagId(34),
                getFlagId(35),
                getFlagId(36),
                getFlagId(37),
                getFlagId(38),
                getFlagId(39),
            ])
        );
        flagStr += flag(
            BitArray([
                getFlagId(40),
                getFlagId(41),
                getFlagId(42),
                getFlagId(43),
                getFlagId(44),
                getFlagId(45),
            ])
        );

        ba = BitArray([getValueFromDropdown("SettingsModel_MagicEffectiveness")]);
        flagStr += flag(
            BitArray([
                getFromBa(ba, 0),
                getFromBa(ba, 1),
                getFromBa(ba, 2),
                getFlagId(46),
                getFlagId(47),
                getFlagId(48),
            ])
        );

        ba = BitArray([
            getValueFromDropdown("SettingsModel_NumberOfPalacesToComplete"),
        ]);
        flagStr += flag(
            BitArray([
                getFlagId(49),
                getFlagId(50),
                getFromBa(ba, 0),
                getFromBa(ba, 1),
                getFromBa(ba, 2),
                getFlagId(51),
            ])
        );

        flagStr += flag(
            BitArray([
                getFlagId(52),
                getFlagId(53),
                getFlagId(54),
                getFlagId(55),
                getFlagId(56),
                getFlagId(57),
            ])
        );

        ba = BitArray([getValueFromDropdown("SettingsModel_LifeEffectiveness")]);
        flagStr += flag(
            BitArray([
                getFromBa(ba, 0),
                getFromBa(ba, 1),
                getFromBa(ba, 2),
                getFlagId(58),
                getFlagId(59),
                getFlagId(60),
            ])
        );

        ba = BitArray([getValueFromDropdown("SettingsModel_MaxHeartContainers")]);
        baTwo = BitArray([getValueFromDropdown("SettingsModel_HiddenPalace")]);
        flagStr += flag(
            BitArray([
                getFromBa(ba, 0),
                getFromBa(ba, 1),
                getFromBa(ba, 2),
                getFromBa(ba, 3),
                getFromBa(baTwo, 0),
                getFromBa(baTwo, 1),
            ])
        );

        ba = BitArray([getValueFromDropdown("SettingsModel_HiddenKasuto")]);
        flagStr += flag(
            BitArray([
                getFromBa(ba, 0),
                getFromBa(ba, 1),
                getFlagId(61),
                getFlagId(62),
                getItemDropValue(0, "Small"),
                getItemDropValue(1, "Small"),
            ])
        );

        flagStr += flag(
            BitArray([
                getItemDropValue(2, "Small"),
                getItemDropValue(3, "Small"),
                getItemDropValue(4, "Small"),
                getItemDropValue(5, "Small"),
                getItemDropValue(6, "Small"),
                getItemDropValue(7, "Small"),
            ])
        );
        flagStr += flag(
            BitArray([
                getItemDropValue(0, "Large"),
                getItemDropValue(1, "Large"),
                getItemDropValue(2, "Large"),
                getItemDropValue(3, "Large"),
                getItemDropValue(4, "Large"),
                getItemDropValue(5, "Large"),
            ])
        );

        ba = BitArray([getValueFromDropdown("SettingsModel_HintType")]);
        flagStr += flag(
            BitArray([
                getItemDropValue(6, "Large"),
                getItemDropValue(7, "Large"),
                getFromBa(ba, 0),
                getFromBa(ba, 1),
                getFlagId(63),
                getFlagId(64),
            ])
        );

        flagStr += flag(BitArray([getFlagId(65), getFlagId(66)]));

        $("#flags").val(flagStr);
    }

    function generateFromFlags(flagsToUse) {
        dontGenerate = true;
        var val = flagsToUse;

        if (flagsToUse === undefined) val = $("#flags").val();

        var v = getBitArray(indexOf(val[0]));
        setValue(1, v[0]);
        setValue(2, v[1]);
        setValue(3, v[2]);
        setValue(4, v[3]);
        setValue(5, v[4]);
        setValue(6, v[5]);

        v = getBitArray(indexOf(val[1]));
        setValue(7, v[0]);
        setValue(8, v[1]);
        setValue(9, v[2]);
        setValue(10, v[3]);
        setValue(11, v[4]);
        setValue(12, v[5]);

        v = getBitArray(indexOf(val[2]));
        setValue(13, v[0]);
        setValue(14, v[1]);
        setValue(15, v[2]);
        setValue(16, v[3]);
        setValue(17, v[4]);
        setValue(18, v[5]);

        v = getBitArray(indexOf(val[3]));
        setValue(19, v[0]);
        setValue(20, v[1]);
        setValue(21, v[2]);
        setValue(22, v[3]);
        setValue(23, v[4]);
        setValue(24, v[5]);

        v = getBitArray(indexOf(val[4]));
        var w = BitArray([
            getRealVal(v[3]),
            getRealVal(v[4]),
            getRealVal(v[5]),
            false,
        ]);
        var arr = Array.from(w);
        setSelectedIndex("#SettingsModel_StartingTechs", arr[0]);

        var x = getBitArray(indexOf(val[5]));
        w = BitArray([
            getRealVal(v[0]),
            getRealVal(v[1]),
            getRealVal(v[2]),
            getRealVal(x[2]),
        ]);
        v = x;
        arr = Array.from(w);
        setSelectedIndex("#SettingsModel_StartingHeartContainers", arr[0]);
        setValue(25, v[0]);
        setValue(26, v[1]);
        setValue(27, v[3]);
        setValue(28, v[4]);
        setValue(29, v[5]);

        v = getBitArray(indexOf(val[6]));
        setValue(30, v[0]);
        setValue(31, v[1]);
        setValue(32, v[5]);
        w = BitArray([getRealVal(v[2]), getRealVal(v[3]), getRealVal(v[4])]);
        arr = Array.from(w);
        setSelectedIndex("#SettingsModel_AttackEffectiveness", arr[0]);

        v = getBitArray(indexOf(val[7]));
        setValue(34, v[0]);
        setValue(35, v[1]);
        setValue(36, v[2]);
        setValue(37, v[3]);
        setValue(38, v[4]);
        setValue(39, v[5]);

        v = getBitArray(indexOf(val[8]));
        setValue(40, v[0]);
        setValue(41, v[1]);
        setValue(42, v[2]);
        setValue(43, v[3]);
        setValue(44, v[4]);
        setValue(45, v[5]);

        v = getBitArray(indexOf(val[9]));
        w = BitArray([getRealVal(v[0]), getRealVal(v[1]), getRealVal(v[2])]);
        arr = Array.from(w);
        setSelectedIndex("#SettingsModel_MagicEffectiveness", arr[0]);
        setValue(46, v[3]);
        setValue(47, v[4]);
        setValue(48, v[5]);

        v = getBitArray(indexOf(val[10]));
        setValue(49, v[0]);
        setValue(50, v[1]);
        w = BitArray([getRealVal(v[2]), getRealVal(v[3]), getRealVal(v[4])]);
        arr = Array.from(w);
        setSelectedIndex("#SettingsModel_NumberOfPalacesToComplete", arr[0]);
        setValue(51, v[5]);

        v = getBitArray(indexOf(val[11]));
        setValue(52, v[0]);
        setValue(53, v[1]);
        setValue(54, v[2]);
        setValue(55, v[3]);
        setValue(56, v[4]);
        setValue(57, v[5]);

        v = getBitArray(indexOf(val[12]));
        w = BitArray([getRealVal(v[0]), getRealVal(v[1]), getRealVal(v[2])]);
        arr = Array.from(w);
        setSelectedIndex("#SettingsModel_LifeEffectiveness", arr[0]);
        setValue(58, v[3]);
        setValue(59, v[4]);
        setValue(60, v[5]);

        v = getBitArray(indexOf(val[13]));
        w = BitArray([
            getRealVal(v[0]),
            getRealVal(v[1]),
            getRealVal(v[2]),
            getRealVal(v[3]),
        ]);
        arr = Array.from(w);
        setSelectedIndex("#SettingsModel_MaxHeartContainers", arr[0]);
        w = BitArray([getRealVal(v[4]), getRealVal(v[5])]);
        arr = Array.from(w);
        setSelectedIndex("#SettingsModel_HiddenPalace", arr[0]);

        v = getBitArray(indexOf(val[14]));
        w = BitArray([getRealVal(v[0]), getRealVal(v[1])]);
        arr = Array.from(w);
        setSelectedIndex("#SettingsModel_HiddenKasuto", arr[0]);
        setValue(61, v[2]);
        setValue(62, v[3]);

        setItemDropVal(0, "Small", v[4]);

        setItemDropVal(1, "Small", v[5]);

        v = getBitArray(indexOf(val[15]));
        setItemDropVal(2, "Small", v[0]);
        setItemDropVal(3, "Small", v[1]);
        setItemDropVal(4, "Small", v[2]);
        setItemDropVal(5, "Small", v[3]);
        setItemDropVal(6, "Small", v[4]);
        setItemDropVal(7, "Small", v[5]);

        v = getBitArray(indexOf(val[16]));
        setItemDropVal(0, "Large", v[0]);
        setItemDropVal(1, "Large", v[1]);
        setItemDropVal(2, "Large", v[2]);
        setItemDropVal(3, "Large", v[3]);
        setItemDropVal(4, "Large", v[4]);
        setItemDropVal(5, "Large", v[5]);

        v = getBitArray(indexOf(val[17]));
        setItemDropVal(6, "Large", v[0]);
        setItemDropVal(7, "Large", v[1]);
        w = BitArray([getRealVal(v[2]), getRealVal(v[3])]);
        arr = Array.from(w);
        setSelectedIndex("#SettingsModel_HintType", arr[0]);
        setValue(63, v[4]);
        setValue(64, v[5]);

        if (val[18] === undefined) {
            setValue(65, false);
            setValue(66, false);
        } else {
            v = getBitArray(indexOf(val[18]));
            var dontProcess = false;
            if (v[0] && v[1])
                dontProcess = true;

            if (!dontProcess) {
                setValue(65, v[0]);
                setValue(66, v[1]);
            }
        }

        updateEverything();
        dontGenerate = false;
        generateFlags();
    }

    function indexOf(char) {
        return flags.indexOf(char);
    }

    function setValue(id, val) {
        $(`.flag${id}`).prop("checked", getRealVal(val));
    }

    function setSelectedIndex(id, val) {
        $(id).val(val);
    }

    function getRealVal(val) {
        if (val === undefined) val = false;
        return val;
    }

    function getBitArray(num) {
        var b = num;
        var a = b ? [] : [false];

        while (b) {
            a.push((b & 1) === 1);
            b >>= 1;
        }

        return a;
    }

    $(".usedForFlags").change(function (e) {
        if (e.originalEvent) {
            generateFlags();
        }
    });
    $("#flags").change(function (e) {
        if (e.originalEvent) {
            generateFromFlags();
        }
    });

    $("#generateBtn").click(function () {
        var seed = Math.round(
            Math.random() * (2147438647 - 1000000000) + 1000000000
        );
        $("#seed").val(seed);
    });

    $(".preset-item").click(function () {
        var clicked = $(this)[0].text;
        switch (clicked) {
            case "Beginner":
                generateFromFlags("jhmhMROm7DZ$cHRBTA");
                break;
            case "Swiss":
                generateFromFlags("jhhhDcM#$Za$LpTBT!");
                break;
            case "Finals":
                generateFromFlags("hhAhC0j#x78gJqTBTR");
                break;
            case "Max":
                generateFromFlags("iyhqh$j#g7@$ZqTBT!");
                break;
            case "Bracket":
                generateFromFlags("hhhhD0j#$Z8$JpTBT!");
                break;
        }
    });

    $("#SettingsModel_ShuffleStartingItems").change(function () {
        shuffleStartingItems();
    });

    $("#SettingsModel_ShuffleStartingSpells").change(function () {
        shuffleStartingSpells();
    });

    $("#SettingsModel_AllowPalacesToSwapContinents").change(function () {
        allowPalacesToSwapContinents();
    });

    $("#SettingsModel_ShuffleEncounters").change(function () {
        shuffleEncounters();
    });

    $("#SettingsModel_ShufflePalaceRooms").change(function () {
        shufflePalaceRooms();
    });

    $("#SettingsModel_RemoveThunderbird").change(function () {
        removeThunderbird();
    });

    $("#SettingsModel_ShuffleAllExperienceNeeded").change(function () {
        shuffleAllExperience();
    });

    $("#SettingsModel_ShuffleOverworldEnemies").change(function () {
        shuffleOverworldEnemies();
    });

    $("#SettingsModel_ShufflePalaceItems").change(function () {
        checkForMixOverworldShuffle();
    });

    $("#SettingsModel_ShuffleOverworldItems").change(function () {
        shuffleOverworldItems();
    });

    $("#SettingsModel_ManuallySelectDrops").change(function () {
        manuallySelectDrops();
    });

    function shuffleStartingItems() {
        if (document.getElementById("SettingsModel_ShuffleStartingItems").checked) {
            $(".startingItem").attr("disabled", true);
            $(".startingItem").prop("checked", false);
        } else {
            $(".startingItem").attr("disabled", false);
        }
    }

    function shuffleStartingSpells() {
        if (
            document.getElementById("SettingsModel_ShuffleStartingSpells").checked
        ) {
            $(".startingSpell").attr("disabled", true);
            $(".startingSpell").prop("checked", false);
        } else {
            $(".startingSpell").attr("disabled", false);
        }
    }

    function allowPalacesToSwapContinents() {
        if (
            document.getElementById("SettingsModel_AllowPalacesToSwapContinents")
                .checked
        ) {
            $(".greatPalaceInShuffle").attr("disabled", false);
        } else {
            $(".greatPalaceInShuffle").attr("disabled", true);
        }
    }

    function shuffleEncounters() {
        if (document.getElementById("SettingsModel_ShuffleEncounters").checked) {
            $(".unsafePathEncounters").attr("disabled", false);
        } else {
            $(".unsafePathEncounters").attr("disabled", true);
        }
    }

    function checkForMixOverworldShuffle() {
        if (
            document.getElementById("SettingsModel_ShuffleOverworldItems").checked &&
            document.getElementById("SettingsModel_ShufflePalaceItems").checked
        ) {
            $(".mixOverworldItems").attr("disabled", false);
        } else {
            $(".mixOverworldItems").attr("disabled", true);
            $(".mixOverworldItems").prop("checked", false);
        }
    }

    function shufflePalaceRooms() {
        if (document.getElementById("SettingsModel_ShufflePalaceRooms").checked) {
            $(".thunderbirdRequired").attr("disabled", false);
            $(".shortenPalace").attr("disabled", false);
        } else {
            $(".thunderbirdRequired").prop("checked", true);
            $(".thunderbirdRequired").attr("disabled", true);
            $(".shortenPalace").prop("checked", false);
            $(".shortenPalace").attr("disabled", true);
        }
    }

    function removeThunderbird() {
        if (document.getElementById("SettingsModel_RemoveThunderbird").checked) {
            $(".thunderbirdRequired").attr("disabled", true);
            $(".thunderbirdRequired").prop("checked", false);
        } else {
            $(".thunderbirdRequired").attr("disabled", false);
        }
    }

    function thunderbirdRequired() {
        if (document.getElementById("SettingsModel_ThunderbirdRequired").checked) {
            $(".removeThunderbird").attr("disabled", true);
            $(".removeThunderbird").prop("checked", false);
        } else {
            $(".removeThunderbird").attr("disabled", false);
        }
    }

    function shuffleAllExperience() {
        if (
            document.getElementById("SettingsModel_ShuffleAllExperienceNeeded")
                .checked
        ) {
            $(".shuffleExperience").attr("disabled", true);
            $(".shuffleExperience").prop("checked", true);
        } else {
            $(".shuffleExperience").attr("disabled", false);
            $(".shuffleExperience").prop("checked", false);
        }
    }

    function shuffleOverworldEnemies() {
        if (
            document.getElementById("SettingsModel_ShuffleOverworldEnemies").checked
        ) {
            $(".mixEnemies").attr("disabled", false);
        } else {
            $(".mixEnemies").attr("disabled", true);
            $(".mixEnemies").prop("checked", false);
        }
    }

    function shuffleOverworldItems() {
        checkForMixOverworldShuffle();
        if (
            document.getElementById("SettingsModel_ShuffleOverworldItems").checked
        ) {
            $(".shufflePbag").attr("disabled", false);
        } else {
            $(".shufflePbag").attr("disabled", true);
            $(".shufflePbag").prop("checked", false);
        }
    }

    function manuallySelectDrops() {
        if (document.getElementById("SettingsModel_ManuallySelectDrops").checked) {
            $(".itemDrops").attr("disabled", false);
            $(".sub-item").show();
        } else {
            $(".itemDrops").attr("disabled", true);
            $(".sub-item").hide();
            $(".itemDrops").prop("checked", false);
            //this is only for a quick fix of an issue with flags I think this is gross
            $("#SmallDropPool_0__Selected").prop("checked", true);
            $("#SmallDropPool_2__Selected").prop("checked", true);
            $("#LargeDropPool_1__Selected").prop("checked", true);
            $("#LargeDropPool_4__Selected").prop("checked", true);
            //force generate flags
            generateFlags();
        }
    }

    $("input:file").change(function () {
        if ($(this).val()) {
            $("input:submit").attr("disabled", false);
        }
    });

    function updateEverything() {
        shuffleStartingItems();
        shuffleStartingSpells();
        allowPalacesToSwapContinents();
        shuffleEncounters();
        checkForMixOverworldShuffle();
        removeThunderbird();
        shufflePalaceRooms();
        thunderbirdRequired();
        shuffleAllExperience();
        shuffleOverworldEnemies();
        shuffleOverworldItems();
        manuallySelectDrops();
    }

    updateEverything();
    generateFlags();
}