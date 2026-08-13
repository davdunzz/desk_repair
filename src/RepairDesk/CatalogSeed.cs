namespace RepairDesk;

public static class CatalogSeed
{
    public static readonly Dictionary<string, string[]> Data = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Apple"] = ["iPhone 5", "iPhone 5s", "iPhone 5c", "iPhone SE (1ª gen)", "iPhone 6", "iPhone 6 Plus", "iPhone 6s", "iPhone 6s Plus", "iPhone 7", "iPhone 7 Plus", "iPhone 8", "iPhone 8 Plus", "iPhone X", "iPhone XR", "iPhone XS", "iPhone XS Max", "iPhone 11", "iPhone 11 Pro", "iPhone 11 Pro Max", "iPhone SE (2ª gen)", "iPhone 12 mini", "iPhone 12", "iPhone 12 Pro", "iPhone 12 Pro Max", "iPhone 13 mini", "iPhone 13", "iPhone 13 Pro", "iPhone 13 Pro Max", "iPhone SE (3ª gen)", "iPhone 14", "iPhone 14 Plus", "iPhone 14 Pro", "iPhone 14 Pro Max", "iPhone 15", "iPhone 15 Plus", "iPhone 15 Pro", "iPhone 15 Pro Max", "iPhone 16e", "iPhone 16", "iPhone 16 Plus", "iPhone 16 Pro", "iPhone 16 Pro Max"],
        ["Samsung"] = ["Galaxy S8", "Galaxy S9", "Galaxy S10", "Galaxy S10e", "Galaxy S20", "Galaxy S20 FE", "Galaxy S21", "Galaxy S21 FE", "Galaxy S22", "Galaxy S23", "Galaxy S23 FE", "Galaxy S24", "Galaxy S24 FE", "Galaxy S25", "Galaxy Note 8", "Galaxy Note 9", "Galaxy Note 10", "Galaxy Note 20", "Galaxy Z Flip3", "Galaxy Z Flip4", "Galaxy Z Flip5", "Galaxy Z Flip6", "Galaxy Z Fold3", "Galaxy Z Fold4", "Galaxy Z Fold5", "Galaxy Z Fold6", "Galaxy A05s", "Galaxy A12", "Galaxy A13", "Galaxy A14", "Galaxy A15", "Galaxy A16", "Galaxy A20e", "Galaxy A21s", "Galaxy A22", "Galaxy A23", "Galaxy A25", "Galaxy A32", "Galaxy A33", "Galaxy A34", "Galaxy A35", "Galaxy A40", "Galaxy A41", "Galaxy A50", "Galaxy A51", "Galaxy A52", "Galaxy A53", "Galaxy A54", "Galaxy A55", "Galaxy A70", "Galaxy A71", "Galaxy A72"],
        ["Xiaomi"] = ["Mi 8", "Mi 9", "Mi 10", "Mi 10 Lite", "Mi 11", "Mi 11 Lite", "Xiaomi 11T", "Xiaomi 11T Pro", "Xiaomi 12", "Xiaomi 12 Lite", "Xiaomi 12T", "Xiaomi 12T Pro", "Xiaomi 13", "Xiaomi 13 Lite", "Xiaomi 13T", "Xiaomi 13T Pro", "Xiaomi 14", "Xiaomi 14T", "Xiaomi 14T Pro", "Xiaomi 15", "Poco F3", "Poco F4", "Poco F5", "Poco F6", "Poco X3", "Poco X4 Pro", "Poco X5", "Poco X6", "Poco M3", "Poco M4", "Poco M5", "Poco M6"],
        ["Redmi"] = ["Redmi 9", "Redmi 9A", "Redmi 9C", "Redmi 10", "Redmi 10C", "Redmi 12", "Redmi 12C", "Redmi 13", "Redmi 13C", "Redmi Note 8", "Redmi Note 9", "Redmi Note 10", "Redmi Note 10 Pro", "Redmi Note 11", "Redmi Note 11 Pro", "Redmi Note 12", "Redmi Note 12 Pro", "Redmi Note 13", "Redmi Note 13 Pro", "Redmi Note 14", "Redmi Note 14 Pro"],
        ["Google"] = ["Pixel 3", "Pixel 4", "Pixel 4a", "Pixel 5", "Pixel 5a", "Pixel 6", "Pixel 6a", "Pixel 6 Pro", "Pixel 7", "Pixel 7a", "Pixel 7 Pro", "Pixel 8", "Pixel 8a", "Pixel 8 Pro", "Pixel 9", "Pixel 9a", "Pixel 9 Pro", "Pixel 9 Pro XL", "Pixel 10", "Pixel 10 Pro", "Pixel 10 Pro XL"],
        ["Huawei"] = ["P20", "P20 Lite", "P20 Pro", "P30", "P30 Lite", "P30 Pro", "P40", "P40 Lite", "P40 Pro", "Mate 10", "Mate 20", "Mate 20 Pro", "Mate 30", "Mate 40 Pro", "Nova 5T", "Nova 8i", "Nova 9", "Nova 10"],
        ["Honor"] = ["Honor 8", "Honor 9", "Honor 10", "Honor 20", "Honor 50", "Honor 70", "Honor 90", "Honor 200", "Magic4 Pro", "Magic5 Pro", "Magic6 Pro", "Magic7 Pro", "X6", "X7", "X8", "X9"],
        ["Motorola"] = ["Moto G5", "Moto G6", "Moto G7", "Moto G8", "Moto G9", "Moto G10", "Moto G20", "Moto G30", "Moto G50", "Moto G52", "Moto G54", "Moto G55", "Moto G60", "Moto G72", "Moto G84", "Moto G85", "Edge 20", "Edge 30", "Edge 40", "Edge 50", "Razr 40", "Razr 50"],
        ["Oppo"] = ["A15", "A16", "A17", "A18", "A38", "A54", "A57", "A58", "A74", "A78", "A94", "A98", "Reno4", "Reno6", "Reno7", "Reno8", "Reno10", "Reno11", "Reno12", "Find X2", "Find X3", "Find X5", "Find X8"],
        ["OnePlus"] = ["OnePlus 5", "OnePlus 6", "OnePlus 7", "OnePlus 8", "OnePlus 9", "OnePlus 10 Pro", "OnePlus 11", "OnePlus 12", "OnePlus 13", "Nord", "Nord 2", "Nord 3", "Nord 4", "Nord CE", "Nord CE 2", "Nord CE 3", "Nord CE 4"],
        ["Realme"] = ["Realme 7", "Realme 8", "Realme 9", "Realme 10", "Realme 11", "Realme 12", "Realme 13", "GT", "GT 2", "GT 3", "GT 5", "GT 6", "C21", "C25", "C31", "C33", "C53", "C55", "C61", "C67", "C75"],
        ["Nothing"] = ["Phone (1)", "Phone (2)", "Phone (2a)", "Phone (2a) Plus", "Phone (3a)", "Phone (3a) Pro"],
        ["Nokia"] = ["Nokia 2", "Nokia 3", "Nokia 5", "Nokia 6", "Nokia 7", "Nokia 8", "Nokia G10", "Nokia G20", "Nokia G21", "Nokia G22", "Nokia G42", "Nokia X10", "Nokia X20", "Nokia X30"],
        ["Sony"] = ["Xperia XZ", "Xperia XZ2", "Xperia 1", "Xperia 1 II", "Xperia 1 III", "Xperia 1 IV", "Xperia 1 V", "Xperia 1 VI", "Xperia 5", "Xperia 5 II", "Xperia 5 III", "Xperia 10", "Xperia 10 V", "Xperia 10 VI"],
        ["Asus"] = ["Zenfone 6", "Zenfone 7", "Zenfone 8", "Zenfone 9", "Zenfone 10", "Zenfone 11 Ultra", "ROG Phone 3", "ROG Phone 5", "ROG Phone 6", "ROG Phone 7", "ROG Phone 8", "ROG Phone 9"],
        ["LG"] = ["G6", "G7 ThinQ", "G8 ThinQ", "V30", "V40", "V50", "V60", "Velvet", "Wing"],
        ["Altro"] = []
    };
}
