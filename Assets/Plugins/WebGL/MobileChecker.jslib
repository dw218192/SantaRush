var MobileChecker = {
   IsMobile: function()
   {
      return Module.SystemInfo.mobile;
   }
};
mergeInto(LibraryManager.library, MobileChecker);