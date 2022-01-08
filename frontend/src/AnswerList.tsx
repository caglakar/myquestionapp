import React from 'react';
import { AnswerData } from './QuestionData';
import { Answer } from './Answer';

interface Props {
  data: AnswerData[];
}

export const AnswerList = ({ data }: Props) => (
  <ul>
    {data.map((a) => (
      <li key={a.answerId}>
        <Answer data={a} />
      </li>
    ))}
  </ul>
);
