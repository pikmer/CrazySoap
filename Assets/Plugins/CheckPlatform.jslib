mergeInto(LibraryManager.library, {
    CheckPlatform: function () { 
        var ua = window.navigator.userAgent.toLowerCase(); 
        return !(ua.indexOf("android") !== -1 || ua.indexOf("iphone") !== -1 || ua.indexOf("ipad") !== -1);
    },
});