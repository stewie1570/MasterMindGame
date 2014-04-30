JSONCasing = {
    toCamel: function (jsonObj)
    {
        return this.helpers.processJsonObj.call(this, {
            jsonObj: jsonObj,
            keyModifier: this.toCamel,
            properFirstCharFunc: function (firstChar) { return firstChar.toLowerCase(); }
        });
    },

    toPascal: function (jsonObj)
    {
        return this.helpers.processJsonObj.call(this, {
            jsonObj: jsonObj,
            keyModifier: this.toPascal,
            properFirstCharFunc: function (firstChar) { return firstChar.toUpperCase(); }
        });
    },

    helpers: {
        processJsonObj: function (jsonCasingCommand)
        {
            var jsonObj = jsonCasingCommand.jsonObj,
                keyModifier = jsonCasingCommand.keyModifier,
                properFirstCharFunc = jsonCasingCommand.properFirstCharFunc,
                replacementObj = this.helpers.isArray(jsonObj) ? [] : {};

            if (this.helpers.isPrimitive(jsonObj))
                return jsonObj;

            for (var key in jsonObj)
            {
                var properKey = this.helpers.properKeyFrom(key, properFirstCharFunc);
                replacementObj[properKey] = this
                    .helpers
                    .recursivelyKeyModifiedJsonObjectFrom.call(this, jsonObj[key], keyModifier);
            }

            return replacementObj;
        },

        isArray: function (obj)
        {
            return Object.prototype.toString.call(obj) == "[object Array]";
        },

        isPrimitive: function (obj)
        {
            return typeof (obj) != "object";
        },

        recursivelyKeyModifiedJsonObjectFrom: function (jsonObj, keyModifier)
        {
            return typeof (jsonObj) == "object" ? keyModifier.call(this, jsonObj) : jsonObj;
        },

        properKeyFrom: function (key, properFirstCharFunc)
        {
            return properFirstCharFunc(key.charAt(0)) + key.substr(1, key.length - 1);
        }
    }
};