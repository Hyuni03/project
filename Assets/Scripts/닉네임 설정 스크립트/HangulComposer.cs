using System.Text;
using System.Collections.Generic;

public class HangulComposer
{
    private StringBuilder buffer = new StringBuilder();

    private char? initial = null;
    private char? medial = null;
    private char? final = null;

    private static readonly string initials = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
    private static readonly string medials = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
    private static readonly string finals = "\0ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

    // 자모 입력
    public void AddJamo(char jamo)
    {
        if (initial == null)
        {
            if (IsInitial(jamo)) initial = jamo;
            else buffer.Append(jamo);
        }
        else if (medial == null)
        {
            if (IsMedial(jamo)) medial = jamo;
            else
            {
                buffer.Append(initial);
                initial = null;
                AddJamo(jamo);
            }
        }
        else if (final == null)
        {
            if (IsFinal(jamo)) final = jamo;
            else
            {
                FlushSyllable();
                AddJamo(jamo);
            }
        }
        else
        {
            FlushSyllable();
            AddJamo(jamo);
        }
    }

    // 백스페이스 처리
    public void Backspace()
    {
        if (final != null) final = null;
        else if (medial != null) medial = null;
        else if (initial != null) initial = null;
        else if (buffer.Length > 0) buffer.Remove(buffer.Length - 1, 1);
    }

    // 현재까지 조합된 결과 반환
    public string GetResult()
    {
        string syllable = ComposeSyllable();
        return buffer.ToString() + syllable;
    }

    // 현재 글자 조합 완료 후 buffer에 추가
    private void FlushSyllable()
    {
        string syllable = ComposeSyllable();
        if (!string.IsNullOrEmpty(syllable))
            buffer.Append(syllable);
        initial = medial = final = null;
    }

    // 조합 알고리즘
    private string ComposeSyllable()
    {
        if (initial == null || medial == null) return "";

        int initialIndex = initials.IndexOf(initial.Value);
        int medialIndex = medials.IndexOf(medial.Value);
        int finalIndex = final != null ? finals.IndexOf(final.Value) : 0;

        if (initialIndex < 0 || medialIndex < 0) return "";

        char syllable = (char)(0xAC00 + (initialIndex * 21 * 28) + (medialIndex * 28) + finalIndex);
        return syllable.ToString();
    }

    private bool IsInitial(char c) => initials.Contains(c);
    private bool IsMedial(char c) => medials.Contains(c);
    private bool IsFinal(char c) => finals.Contains(c);
}
