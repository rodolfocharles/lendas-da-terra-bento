from collections import deque
from PIL import Image, ImageDraw
import os

src_dir = r"C:\Users\rosdo\.cursor\projects\c-Users-rosdo-PROJETOS-JOGOS-LendasDaTerraBento\assets\iara_raw"
out_dir = r"c:\Users\rosdo\PROJETOS\JOGOS\LendasDaTerraBento\Assets\_Project\Art\Personagens\Iara"
preview_dir = r"C:\Users\rosdo\.cursor\projects\c-Users-rosdo-PROJETOS-JOGOS-LendasDaTerraBento\assets\iara_preview"

os.makedirs(out_dir, exist_ok=True)
os.makedirs(os.path.join(out_dir, "Retratos"), exist_ok=True)
os.makedirs(preview_dir, exist_ok=True)

files = [
    ("Iara_Bom.png", "Iara_Bom.png", "Iara_Retrato_Bom.png"),
    ("Iara_Triste.png", "Iara_Triste.png", "Iara_Retrato_Triste.png"),
    ("Iara_Calmo.png", "Iara_Calmo.png", "Iara_Retrato_Calmo.png"),
    ("Iara_Feliz.png", "Iara_Feliz.png", "Iara_Retrato_Feliz.png"),
    ("Iara_Corrupto.png", "Iara_Corrupto.png", "Iara_Retrato_Corrupto.png"),
]


def add_outline(img, color=(28, 18, 22, 255)):
    w, h = img.size
    src = img.load()
    out = img.copy()
    dst = out.load()
    for y in range(h):
        for x in range(w):
            if src[x, y][3] != 0:
                continue
            edge = False
            for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                nx, ny = x + dx, y + dy
                if 0 <= nx < w and 0 <= ny < h and src[nx, ny][3] > 0:
                    edge = True
                    break
            if edge:
                dst[x, y] = color
    return out


def crop_opaque(img, pad=2):
    bbox = img.getbbox()
    if not bbox:
        return img
    l, t, r, b = bbox
    l = max(0, l - pad)
    t = max(0, t - pad)
    r = min(img.size[0], r + pad)
    b = min(img.size[1], b + pad)
    return img.crop((l, t, r, b))


def fit_square(img, size):
    cut = crop_opaque(img)
    w, h = cut.size
    side = max(w, h, 1)
    canvas = Image.new("RGBA", (side, side), (0, 0, 0, 0))
    canvas.paste(cut, ((side - w) // 2, (side - h) // 2), cut)
    return canvas.resize((size, size), Image.Resampling.NEAREST)


def head_crop(img, size=64):
    cut = crop_opaque(img, 1)
    w, h = cut.size
    head_h = max(16, int(h * 0.48))
    head = cut.crop((0, 0, w, head_h))
    canvas = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    hw, hh = head.size
    scale = min((size - 8) / max(hw, 1), (size * 0.7) / max(hh, 1))
    nw, nh = max(1, int(hw * scale)), max(1, int(hh * scale))
    head = head.resize((nw, nh), Image.Resampling.NEAREST)
    canvas.paste(head, ((size - nw) // 2, (size - nh) // 2 + 4), head)
    return add_outline(canvas)


def draw_vitoria_regia(size=64):
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    cx, cy = size // 2, size // 2 + 6
    d.ellipse((6, 18, 58, 58), fill=(28, 92, 48, 255))
    d.ellipse((9, 21, 55, 55), fill=(46, 130, 62, 255))
    d.ellipse((14, 26, 50, 52), fill=(62, 150, 72, 255))
    d.polygon([(32, 38), (20, 28), (32, 24), (44, 28)], fill=(38, 110, 54, 255))
    d.ellipse((24, 22, 40, 38), fill=(236, 118, 158, 255))
    d.ellipse((26, 24, 38, 36), fill=(250, 176, 196, 255))
    d.ellipse((29, 27, 35, 33), fill=(250, 214, 96, 255))
    d.point((31, 29), fill=(120, 72, 24, 255))
    return add_outline(img, (18, 42, 28, 255))


for src_name, sprite_name, retrato_name in files:
    path = os.path.join(src_dir, src_name)
    if not os.path.exists(path):
        print("missing", src_name)
        continue
    raw = Image.open(path).convert("RGBA")
    sprite64 = add_outline(raw)
    retrato96 = add_outline(fit_square(raw, 96))
    sprite64.save(os.path.join(out_dir, sprite_name))
    retrato96.save(os.path.join(out_dir, "Retratos", retrato_name))
    sprite64.resize((256, 256), Image.Resampling.NEAREST).save(
        os.path.join(preview_dir, sprite_name.replace(".png", "_x4.png"))
    )
    print("saved", sprite_name)

agua_src = os.path.join(src_dir, "Iara_Corrupto.png")
if not os.path.exists(agua_src):
    agua_src = os.path.join(src_dir, "Iara_Bom.png")
if os.path.exists(agua_src):
    agua = head_crop(Image.open(agua_src).convert("RGBA"))
    agua.save(os.path.join(out_dir, "Iara_Agua.png"))
    agua.resize((256, 256), Image.Resampling.NEAREST).save(
        os.path.join(preview_dir, "Iara_Agua_x4.png")
    )
    print("saved Iara_Agua.png")

folha_src = os.path.join(src_dir, "VitoriaRegia.png")
if os.path.exists(folha_src):
    folha = add_outline(Image.open(folha_src).convert("RGBA"))
else:
    folha = draw_vitoria_regia(64)
folha.save(os.path.join(out_dir, "VitoriaRegia.png"))
folha.resize((256, 256), Image.Resampling.NEAREST).save(
    os.path.join(preview_dir, "VitoriaRegia_x4.png")
)
print("saved VitoriaRegia.png")
print("done")
