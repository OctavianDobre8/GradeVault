import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ClassesRoutingModule } from './classes-routing.module';
import { ClassViewComponent } from './class-view/class-view.component';
import { ClassGradesComponent } from '../grades/class-grades/class-grades.component';
import { ClassesService } from '../../core/services/classes.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ClassesRoutingModule,
    ClassViewComponent,
    ClassGradesComponent,
  ],
  providers: [ClassesService],
})
export class ClassesModule {}
