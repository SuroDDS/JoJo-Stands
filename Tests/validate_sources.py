"""Offline structural checks; not a replacement for compiling or running RimWorld."""
from pathlib import Path
import re
from xml.etree import ElementTree as ET

root = Path(__file__).resolve().parents[1]
xml_files = list(root.rglob('*.xml'))
assert xml_files, 'No XML files'
for filename in xml_files:
    ET.parse(filename)

sources = '\n'.join(p.read_text(encoding='utf-8') for p in (root / 'Source').rglob('*.cs'))
stand_defs = ET.parse(root / 'Defs/StandDefs/Stands.xml').getroot()
arrow_defs = ET.parse(root / 'Defs/ThingDefs/StandArrow.xml').getroot()
hediff_defs = ET.parse(root / 'Defs/HediffDefs/StandBond.xml').getroot()
assert any(x.findtext('defName') == 'JJ_StarPlatinum' for x in stand_defs)
assert any(x.findtext('defName') == 'JJ_StandArrow' for x in arrow_defs)
assert any(x.findtext('defName') == 'JJ_StandBond' for x in hediff_defs)
assert (root / 'Textures/Things/StandArrow.png').exists()

code_keys = set(re.findall(r'"(JJ_[A-Za-z0-9_]+)"\.Translate\(', sources))
locale_files = [root / 'Languages/English/Keyed/JoJoStand.xml', root / 'Languages/Russian/Keyed/JoJoStand.xml']
for loc in locale_files:
    keys = {node.tag for node in ET.parse(loc).getroot()}
    missing = code_keys - keys
    assert not missing, f'Missing {loc}: {sorted(missing)}'

assert '<TargetFramework>net10.0</TargetFramework>' in (root / 'Source/JoJoStand/JoJoStand.csproj').read_text()
assert not list((root / '1.7/Assemblies').glob('0Harmony.dll')), 'Never bundle Harmony.'
print(f'Structure checks passed: {len(xml_files)} XML files, {len(code_keys)} localisation keys, Stand Arrow texture present.')
print('NOT checked: C# compilation, assembly references, Harmony patching, RimWorld runtime.')
