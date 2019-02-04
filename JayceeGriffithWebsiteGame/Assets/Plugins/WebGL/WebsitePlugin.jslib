mergeInto(LibraryManager.library, {
  SetPage: function(page) {
    ReactUnityWebGL.SetPage(Pointer_stringify(page));
  },
  OnReady: function(page) {
    ReactUnityWebGL.OnReady();
  },
  GetPageNavigation: function() {
    ReactUnityWebGL.GetPageNavigation();
  },
  GetPageWidth: function() {
    ReactUnityWebGL.GetPageWidth();
  }
});