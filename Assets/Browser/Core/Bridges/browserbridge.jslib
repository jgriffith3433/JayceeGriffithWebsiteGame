var LibraryBrowserModule = {
    DispatchPacket: function (browserPacket) {
		  EngineTrigger.dispatch("browser_packet", JSON.parse(UTF8ToString(browserPacket)));
    }
};

mergeInto(LibraryManager.library, LibraryBrowserModule);