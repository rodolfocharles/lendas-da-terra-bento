from collections import deque
from PIL import Image, ImageEnhance
import os

src_dir = r"C:\Users\rosdo\.cursor\projects\c-Users-rosdo-PROJETOS-JOGOS-LendasDaTerraBento\assets"
out_dir = r"c:\Users\rosdo\PROJETOS\JOGOS\LendasDaTerraBento\Assets\_Project\Art\Personagens\Boto"
preview_dir = os.path.join(src_dir, "boto_preview")

os.makedirs(out_dir, exist_ok=True)
os.makedirs(os.path.join(out_dir, "Retratos"), exist_ok=True)
os.makedirs(preview_dir, exist_ok=True)

files = [
    ("Boto_Bom.jpg", "Boto_Bom.png", "Boto_Retrato_Bom.png"),
    ("Boto_Triste.jpg", "Boto_Triste.png", "Boto_Retrato_Triste.png"),
    ("Boto_Calmo.jpg", "Boto_Calmo.png", "Boto_Retrato_Calmo.png"),
    ("Boto_Feliz.jpg", "Boto_Feliz.png", "Boto_Retrato_Feliz.png"),
    ("Boto_Corrupto.jpg", "Boto_Corrupto.png", "Boto_Retrato_Corrupto.png"),
]


def flood_alpha(img, limit=14):
    img = img.convert("RGBA")
    w, h = img.size
    px = img.load()
    visited = set()
    q = deque()

    def dark(x, y):
        r, g, b, a = px[x, y]
        return r < limit and g < limit and b < limit

    for x in range(w):
        q.append((x, 0))
        q.append((x, h - 1))
    for y in range(h):
        q.append((0, y))
        q.append((w - 1, y))

    while q:
        x, y = q.popleft()
        if x < 0 or y < 0 or x >= w or y >= h or (x, y) in visited:
            continue
        visited.add((x, y))
        if not dark(x, y):
            continue
        r, g, b, a = px[x, y]
        px[x, y] = (r, g, b, 0)
        q.append((x + 1, y))
        q.append((x - 1, y))
        q.append((x, y + 1))
        q.append((x, y - 1))
    return img


def crop(img, pad_ratio=0.05):
    bbox = img.getbbox()
    if not bbox:
        return img
    l, t, r, b = bbox
    w, h = img.size
    pad = int(max(r - l, b - t) * pad_ratio)
    l = max(0, l - pad)
    t = max(0, t - pad)
    r = min(w, r + pad)
    b = min(h, b + pad)
    cw, ch = r - l, b - t
    side = max(cw, ch)
    cx, cy = (l + r) // 2, (t + b) // 2
    l2 = max(0, cx - side // 2)
    t2 = max(0, cy - side // 2)
    r2 = min(w, l2 + side)
    b2 = min(h, t2 + side)
    if r2 - l2 < side:
        l2 = max(0, r2 - side)
    if b2 - t2 < side:
        t2 = max(0, b2 - side)
    return img.crop((l2, t2, r2, b2))


def fit_square(img, size):
    w, h = img.size
    side = max(w, h)
    canvas = Image.new("RGBA", (side, side), (0, 0, 0, 0))
    canvas.paste(img, ((side - w) // 2, (side - h) // 2), img)
    small = canvas.resize((size, size), Image.Resampling.BOX)
    alpha = small.split()[-1]
    rgb = small.convert("RGB").quantize(colors=28, method=Image.Quantize.MEDIANCUT).convert("RGB")
    out = rgb.convert("RGBA")
    out.putalpha(alpha.point(lambda a: 255 if a >= 96 else 0))
    return out


def add_outline(img, color=(42, 24, 28, 255)):
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


for src_name, sprite_name, retrato_name in files:
    raw = Image.open(os.path.join(src_dir, src_name))
    limit = 10 if "Corrupto" in src_name else 16
    cut = flood_alpha(raw, limit)
    cut = crop(cut, 0.05)
    if "Corrupto" in src_name:
        cut = ImageEnhance.Brightness(cut).enhance(1.45)
        cut = ImageEnhance.Contrast(cut).enhance(1.35)
    sprite64 = add_outline(fit_square(cut, 64))
    retrato96 = add_outline(fit_square(cut, 96))
    sprite64.save(os.path.join(out_dir, sprite_name))
    retrato96.save(os.path.join(out_dir, "Retratos", retrato_name))
    sprite64.resize((256, 256), Image.Resampling.NEAREST).save(
        os.path.join(preview_dir, sprite_name.replace(".png", "_x4.png"))
    )
    print("saved", sprite_name, sprite64.size)

print("done")
