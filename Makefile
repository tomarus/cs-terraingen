MOD = TerrainGen

BASE=$(HOME)/Library/Application Support/Steam/SteamApps/common/Cities_Skylines/Cities.app/Contents
INST=$(HOME)/Library/Application Support/Colossal Order/Cities_Skylines/Addons/Mods

# Use mono from game to prevent version conflicts.
MONO="$(BASE)/Frameworks/Mono/bin/mono"
MCS="$(BASE)/Frameworks/Mono/lib/mono/2.0/gmcs.exe"

DLL = bin/Debug/$(MOD).dll

.PHONY: clean all

REFS = $(filter-out $(DLL).dll,$(shell find . -name '*.dll'))
SRCS = $(shell find . -name '*.cs')

all:	$(DLL)

%.dll:	$(SRCS)
	MONO_PATH="$(BASE)/Resources/Data/Managed" $(MONO) $(MCS) $(MCSFLAGS) -target:library -out:$@ $(filter %.cs,$^) $(addprefix -r:,$(REFS))

clean:
	@rm -f $(DLL)

install:
	mkdir -p "$(INST)/$(MOD)"
	cp $(DLL) "$(INST)/$(MOD)"

refs:
	mkdir -p bin/Debug
	cp -a "$(BASE)/Resources/Data/Managed/"*.dll bin/Debug
