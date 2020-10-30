function is_mobile(e) {
    e.length > 11 && (e = e.slice(-11));
    var t = /^(\+[0-9]{2,}-?)?1[0-9]{10}$/;
    return t.test(e)
}
function get_a_random() {
    var e = new Array("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f");
    return String(e[parseInt(16 * Math.random(), 10)])
}
function pre_fix_integer(e, t) {
    return (Array(t).join(0) + e).slice(-t)
}
function encode_mobile(e) {
    if (is_mobile(e)) {
        var t = "abcdef",
		e = String(e);
        e = e.substring(0, 2) + get_a_random() + get_a_random() + e.substring(2, 5) + get_a_random() + get_a_random() + e.substring(5, 8) + get_a_random() + e.substring(8, 11);
        var n = String(parseInt("0x" + String(e.substring(0, 4))) ^ t),
		i = String(parseInt("0x" + String(e.substring(4, 8))) ^ t),
		r = String(parseInt("0x" + String(e.substring(8, 12))) ^ t),
		o = String(parseInt("0x" + String(e.substring(12, 16))) ^ t);
        return r + "-" + o + "-" + n + "-" + i
    }
    return e
}
function decode_mobile(e) {
    var t = "abcdef";
    e = e.split("-");
    var n = pre_fix_integer(Number(e[0] ^ t).toString(16), 4),
	i = pre_fix_integer(Number(e[1] ^ t).toString(16), 4),
	r = pre_fix_integer(Number(e[2] ^ t).toString(16), 4),
	o = pre_fix_integer(Number(e[3] ^ t).toString(16), 4),
	a = r + o + n + i;
    return a.substring(0, 2) + a.substring(4, 7) + a.substring(9, 12) + a.substring(13, 17)
}