# -*- mode: python ; coding: utf-8 -*-


a = Analysis(
    ['youtube_downloader.py'],
    pathex=[],
    binaries=[],
    datas=[('C:\\Users\\redxx\\AppData\\Local\\Programs\\Python\\Python312\\tcl\\tcl8.6', 'tcl\\tcl8.6'), ('C:\\Users\\redxx\\AppData\\Local\\Programs\\Python\\Python312\\tcl\\tk8.6', 'tcl\\tk8.6')],
    hiddenimports=[],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    noarchive=False,
)
pyz = PYZ(a.pure)

exe = EXE(
    pyz,
    a.scripts,
    a.binaries,
    a.datas,
    [],
    name='YouTube Downloader',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    upx_exclude=[],
    runtime_tmpdir=None,
    console=False,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
