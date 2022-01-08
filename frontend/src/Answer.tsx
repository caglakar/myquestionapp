import React from 'react';
import { AnswerData } from './QuestionData';

interface Props {
  data: AnswerData;
}

export const Answer = ({ data }: Props) => (
  <div>
    <div>{data.content}</div>
    <div>
      {`Answered by ${
        data.userName
      } on ${data.created.toLocaleDateString()} ${data.created.toLocaleTimeString()}`}
    </div>
  </div>
);
