using System.Text;
using System.Collections.Generic;

public class HangulComposer
{
    private StringBuilder buffer = new StringBuilder();

    private char? initial = null;
    private char? medial = null;
    private char? final = null;

    private static readonly string initials = "��������������������������������������";
    private static readonly string medials = "�������¤äĤŤƤǤȤɤʤˤ̤ͤΤϤФѤҤ�";
    private static readonly string finals = "\0������������������������������������������������������";

    // �ڸ� �Է�
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

    // �齺���̽� ó��
    public void Backspace()
    {
        if (final != null) final = null;
        else if (medial != null) medial = null;
        else if (initial != null) initial = null;
        else if (buffer.Length > 0) buffer.Remove(buffer.Length - 1, 1);
    }

    // ������� ���յ� ��� ��ȯ
    public string GetResult()
    {
        string syllable = ComposeSyllable();
        return buffer.ToString() + syllable;
    }

    // ���� ���� ���� �Ϸ� �� buffer�� �߰�
    private void FlushSyllable()
    {
        string syllable = ComposeSyllable();
        if (!string.IsNullOrEmpty(syllable))
            buffer.Append(syllable);
        initial = medial = final = null;
    }

    // ���� �˰���
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
